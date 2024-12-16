using Microsoft.AspNetCore.Components.Forms;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Models
{
    public class RevitFile : IBrowserFile
    {
        private readonly IBrowserFile _browserFile;


        public RevitFile(IBrowserFile browserFile)
        {
            _browserFile = browserFile;
            Status = FileStatus.GetVersion;
        }

        public string Name
        {
            get { return _browserFile.Name; }
        }

        public string DownloadUrl { get; set; }

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

        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private FileStatus _status;
        public FileStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public event EventHandler<EventArgs> StatusChanged;
        public event EventHandler<EventArgs> FileRemoved;

        private void RaiseStatusChanged()
        {
            var handler = StatusChanged;
            if (handler == null)
                return;

            handler(this, new EventArgs());
        }

        public void RaiseFileRemoved()
        {
            var handler = FileRemoved;
            if (handler == null)
                return;

            handler(this, new EventArgs());
        }

        public async Task UploadFile(IDataService dataService, IUploadService uploadService)
        {
            Status = FileStatus.Uploading;

            // Upload the files here
            string objectName = System.Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "");
            string objectKey = objectName; // + Path.GetExtension(revitFile.Name);

            int chunksNumber = CalculateNumberOfChunks((ulong)Size);

            long maxFileSize = 1024 * 1024 * 600; // 600 MB

            Signeds3uploadResponse signedsUrlResponse = await dataService.GetUploadUrls(chunksNumber, objectKey);
            long start = 0;
            long chunkSize = Constants.ChunkSize;
            chunkSize = (chunksNumber > 1 ? chunkSize : Size);
            long end = chunkSize;


            //3 upload chunk one by one (or in parallel) 
            // make record with eTag array 
            List<string> eTags = new List<string>();

            MemoryStream ms = new MemoryStream();
            await OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(ms);

            for (int chunkIndex = 0; chunkIndex < chunksNumber; chunkIndex++)
            {
                long numberOfBytes = chunkSize + 1;
                byte[] fileBytes = new byte[numberOfBytes];
                MemoryStream memoryStream = new MemoryStream(fileBytes);
                ms.Seek((int)start, SeekOrigin.Begin);
                int count = ms.Read(fileBytes, 0, (int)numberOfBytes);
                memoryStream.Write(fileBytes, 0, (int)numberOfBytes);
                memoryStream.Position = 0;

                string eTag = await uploadService.UploadChunk(memoryStream, signedsUrlResponse.Urls[chunkIndex]);

                eTags.Add(eTag);
                start = end + 1;
                chunkSize = ((start + chunkSize > Size) ? Size - start - 1 : chunkSize);
                end = start + chunkSize;
            }
            if (eTags.Count == signedsUrlResponse.Urls.Count)
            {

                //4. tell Forge to complete the uploading
                CompleteUploadResponse completeUploadResponse = await dataService.CompleteUpload(
                    signedsUrlResponse.UploadKey, Size, eTags, objectKey);

                Status = FileStatus.Converting;


                //5 Create a workItem
                WorkItemStatus createdWorkItemStatus = await dataService.CreateWorkItem(objectKey, Version, Name);

                string workItemId = createdWorkItemStatus.Id;

                while (true)
                {
                    // 6 Get workitem status
                    WorkItemStatus status = await dataService.GetWorkItemStatus(workItemId);

                    switch (status.Status)
                    {
                        case Models.Status.Pending:
                            break;
                        case Models.Status.Inprogress:
                            break;
                        case Models.Status.Cancelled:
                            break;
                        case Models.Status.FailedLimitDataSize:
                            Status = FileStatus.Error("FailedLimitDataSize");
                            return;
                        case Models.Status.FailedLimitProcessingTime:
                            Status = FileStatus.Error("FailedLimitProcessingTime");
                            return;
                        case Models.Status.FailedDownload:
                            Status = FileStatus.Error("FailedDownload");
                            return;
                        case Models.Status.FailedInstructions:
                            Status = FileStatus.Error("FailedInstructions");
                            return;
                        case Models.Status.FailedUpload:
                            Status = FileStatus.Error("FailedUpload");
                            return;
                        case Models.Status.FailedUploadOptional:
                            Status = FileStatus.Error("FailedUploadOptional");
                            return;
                        case Models.Status.Success:
                            Signeds3downloadResponse signedDownload = await dataService.GetDownloadUrl(objectKey, Name);
                            DownloadUrl = signedDownload?.Url;
                            Status = FileStatus.Converted;
                            return;
                        default:
                            Status = FileStatus.Error("Unknonw error");
                            return;
                    }

                    await Task.Delay(5000);
                }
            }
            else
            {
                Console.WriteLine("[some chunks stream uploading] failed ");
            }

            Status = FileStatus.Converted;
        }


        private int CalculateNumberOfChunks(ulong fileSize)
        {
            if (fileSize == 0)
            {
                return 1;
            }

            int numberOfChunks = (int)Math.Truncate((double)(fileSize / Constants.ChunkSize));
            if (fileSize % Constants.ChunkSize != 0)
            {
                numberOfChunks++;
            }

            return numberOfChunks;
        }

    }

    static class Constants
    {
        public const int MaxRetry = 5;
        public const long ChunkSize = 5 * 1024 * 1024;
        public const int BatchSize = 25;
    }
}