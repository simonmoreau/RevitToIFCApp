using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Charts;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static MudBlazor.CategoryTypes;
using System.IO.Packaging;

namespace WebClient.Models
{
    public class RevitFile : IBrowserFile
    {
        private readonly IBrowserFile _browserFile;
        

        public RevitFile(IBrowserFile browserFile)
        {
            _browserFile= browserFile;
            
        }

        public string Name
        {
            get { return _browserFile.Name; }
        }

        public DateTimeOffset LastModified
        {
            get { return _browserFile.LastModified; }
        }

        public long Size
        {
            get { return _browserFile.Size; }
        }

        public string ContentType
        {
            get { return _browserFile.ContentType; }
        }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return _browserFile.OpenReadStream(maxAllowedSize, cancellationToken);
        }

        private string _part;
        public string Part
        {
            get { return _part; }
            set {  _part = value; }
        }

        public async Task ReadPart()
        {
            if (!string.IsNullOrEmpty(Part)) return;

            long maxFileSize = 1024 * 1024 * 5; // 5 MB

            string path = Path.GetTempFileName();
            await using FileStream fs = new(path, FileMode.Create);
            await _browserFile.OpenReadStream(maxFileSize).CopyToAsync(fs);

            var result = new StringBuilder();

            int i = 0;
            string? line;

            

            var readStream = _browserFile.OpenReadStream(maxFileSize);

            var buf = new byte[readStream.Length];

            var ms = new MemoryStream(buf);

            await readStream.CopyToAsync(ms);

            var buffer = ms.ToArray();

            string test = System.Text.Encoding.UTF8.GetString(buffer,0, 1024 * 1024);
            // string test =  Convert.ToBase64String(buffer);
            _part = test.Replace("\0", string.Empty);


            return;

            using (var reader = new StreamReader(_browserFile.OpenReadStream(maxFileSize)))
            {
                while ((line = await reader.ReadLineAsync()) != null && i < 100)
                {
                    if (line == null) continue;
                    line = line.Replace("\0", string.Empty);
                    result.AppendLine(line);
                    i++;
                }
            }

            Part = i.ToString() + "\r\n" + result.ToString();

            return;

            string fileAsString = result.ToString();

            string[] patterns = { 
                "Format: (.*) ",
                "Revit Build: Autodesk Revit (\\d+)",
                "Revit Build: Autodesk Revit Architecture (\\d+)",
                "Revit Build: Autodesk Revit MEP (\\d+)",
                "Revit Build: Autodesk Revit Structure (\\d+)"
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(fileAsString, pattern);
                if (match.Success)
                {
                    Part = match.Groups[1].Value;
                    break;
                }
            }


            // return result;

            //const int chunkSize = 1024; // read the file by chunks of 1KB
            //using (var file = _browserFile.OpenReadStream(maxAllowedSize:35035840))
            //{
            //    int bytesRead;
            //    var buffer = new byte[chunkSize];
            //    while ((bytesRead = await file.ReadAsync(buffer, 0, buffer.Length)) > 0)
            //    {
            //        // TODO: Process bytesRead number of bytes from the buffer
            //        // not the entire buffer as the size of the buffer is 1KB
            //        // whereas the actual number of bytes that are read are 
            //        // stored in the bytesRead integer.
            //        try
            //        {
            //            string temp = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            //            Console.WriteLine("test");
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //            throw;
            //        }

            //    }
            //}
        }

        private IEnumerable<byte[]> ReadChunks()
        {
            byte[] lengthBytes = new byte[sizeof(int)];

            using (FileStream fileStream = (FileStream)_browserFile.OpenReadStream())
            {
                int n = fileStream.Read(lengthBytes, 0, sizeof(int));  // Read block size.

                if (n == 0)      // End of file.
                    yield break;

                if (n != sizeof(int))
                    throw new InvalidOperationException("Invalid header");

                int blockLength = BitConverter.ToInt32(lengthBytes, 0);
                byte[] buffer = new byte[blockLength];
                n = fileStream.Read(buffer, 0, blockLength);

                if (n != blockLength)
                    throw new InvalidOperationException("Missing data");

                yield return buffer;
            }
        }

        private string FindRevitVersionInText(string contents)
        {
            return "test";
            string line = contents.ToString().Replace("[^ -~]+", "");
            if (line.Contains("Format:"))
            {
                Regex regex = new Regex("Format: (\\d+)");
                Match found = regex.Match(line);
                string version = found.Groups[1].Value.Replace("Format: ", "");
                Console.WriteLine(version);
                return version;
            }
            else if (line.Contains("Revit Build: "))
            {
                Regex regex = new Regex("Revit Build: Autodesk Revit (\\d+)");
                Match found = regex.Match(line);
                string version = null;
                if (found.Success)
                {
                    version = found.Groups[1].Value.Replace("Revit Build: Autodesk Revit ", "");
                }
                else
                {
                    if (line.Contains("Architecture"))
                    {
                        regex = new Regex("Revit Build: Autodesk Revit Architecture (\\d+)");
                        found = regex.Match(line);
                        if (found.Success)
                        {
                            version = found.Groups[1].Value.Replace("Revit Build: Autodesk Revit Architecture ", "");
                        }
                    }
                    else if (line.Contains("MEP"))
                    {
                        regex = new Regex("Revit Build: Autodesk Revit MEP (\\d+)");
                        found = regex.Match(line);
                        if (found.Success)
                        {
                            version = found.Groups[1].Value.Replace("Revit Build: Autodesk Revit MEP ", "");
                        }
                    }
                    else if (line.Contains("Structure"))
                    {
                        regex = new Regex("Revit Build: Autodesk Revit Structure (\\d+)");
                        found = regex.Match(line);
                        if (found.Success)
                        {
                            version = found.Groups[1].Value.Replace("Revit Build: Autodesk Revit Structure ", "");
                        }
                    }
                }
            }
        }


    }
}
