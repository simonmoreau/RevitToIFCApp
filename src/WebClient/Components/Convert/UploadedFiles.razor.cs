using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OpenMcdf;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Text;
using WebClient.Models;
using WebClient.Services;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace WebClient.Components.Convert
{
    public partial class UploadedFiles
    {
        [Inject]
        public IDataService _dataService { get; set; }

        [Inject]
        public IUploadService _uploadService { get; set; }

        private bool _revitFilesNeedUpdate =false;

        [Parameter]
        public ObservableCollection<RevitFile> BrowserFiles { get; set; } = default!;

        protected override void OnInitialized()
        {
            BrowserFiles.CollectionChanged += BrowserFiles_CollectionChanged;   
        }

        private void BrowserFiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _revitFilesNeedUpdate = true;
        }

        private void RemoveFile(RevitFile revitFile)
        {
            BrowserFiles.Remove(revitFile);
        }

        private async Task UploadFile(RevitFile revitFile)
        {
            revitFile.Status = FileStatus.Uploading;
            // Upload the files here
            string objectName = System.Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "");
            string objectKey = objectName; // + Path.GetExtension(revitFile.Name);

            int chunksNumber = CalculateNumberOfChunks((ulong)revitFile.Size);

            long maxFileSize = 1024 * 1024 * 600; // 600 MB

            Signeds3uploadResponse signedsUrlResponse = await _dataService.GetUploadUrls(chunksNumber, objectKey);
            long start = 0;
            long chunkSize = Constants.ChunkSize;
            chunkSize = (chunksNumber > 1 ? chunkSize : revitFile.Size);
            long end = chunkSize;


            //3 upload chunk one by one (or in parallel) 
            // make record with eTag array 
            List<string> eTags = new List<string>();

            MemoryStream ms = new MemoryStream();
            await revitFile.OpenReadStream(maxAllowedSize:maxFileSize).CopyToAsync(ms);

            for (int chunkIndex = 0; chunkIndex < chunksNumber; chunkIndex++)
            {
                long numberOfBytes = chunkSize + 1;
                byte[] fileBytes = new byte[numberOfBytes];
                MemoryStream memoryStream = new MemoryStream(fileBytes);
                ms.Seek((int)start, SeekOrigin.Begin);
                int count = ms.Read(fileBytes, 0, (int)numberOfBytes);
                memoryStream.Write(fileBytes, 0, (int)numberOfBytes);
                memoryStream.Position = 0;

                string eTag = await _uploadService.UploadChunk(memoryStream, signedsUrlResponse.Urls[chunkIndex]);

                eTags.Add(eTag);
                start = end + 1;
                chunkSize = ((start + chunkSize > revitFile.Size) ? revitFile.Size - start - 1 : chunkSize);
                end = start + chunkSize;
            }
            if (eTags.Count == signedsUrlResponse.Urls.Count)
            {
                revitFile.Status = FileStatus.Converting;
                //4. tell Forge to complete the uploading

                CompleteUploadResponse completeUploadResponse = await _dataService.CompleteUpload(
                    signedsUrlResponse.UploadKey, revitFile.Size, eTags, objectKey);

                //5 Create a workItem
                WorkItemStatus createdWorkItemStatus = await _dataService.CreateWorkItem(objectKey, "RevitToIfc_dev.RvtToIfcActivity+Dev");

                string workItemId = createdWorkItemStatus.Id;

                while (true)
                {
                    // 6 Get workitem status
                    WorkItemStatus status = await _dataService.GetWorkItemStatus(workItemId);

                    switch (status.Status)
                    {
                        case Status.Pending:
                            break;
                        case Status.Inprogress:
                            break;
                        case Status.Cancelled:
                            break;
                        case Status.FailedLimitDataSize:
                            revitFile.Status = FileStatus.Error("FailedLimitDataSize");
                            return;
                        case Status.FailedLimitProcessingTime:
                            revitFile.Status = FileStatus.Error("FailedLimitProcessingTime");
                            return;
                        case Status.FailedDownload:
                            revitFile.Status = FileStatus.Error("FailedDownload");
                            return;
                        case Status.FailedInstructions:
                            revitFile.Status = FileStatus.Error("FailedInstructions");
                            return;
                        case Status.FailedUpload:
                            revitFile.Status = FileStatus.Error("FailedUpload");
                            return;
                        case Status.FailedUploadOptional:
                            revitFile.Status = FileStatus.Error("FailedUploadOptional");
                            return;
                        case Status.Success:
                            Signeds3downloadResponse signedDownload = await _dataService.GetDownloadUrl(objectKey);
                            revitFile.DownloadUrl = signedDownload.Url;
                            revitFile.Status = FileStatus.Converted;
                            return;
                        default:
                            revitFile.Status = FileStatus.Error("Unknonw error");
                            return;
                    }

                    await Task.Delay(5000);
                }
            }
            else
            {
                Console.WriteLine("[some chunks stream uploading] failed ");
            } 

            revitFile.Status = FileStatus.Converted;

            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("TODO: Upload your files!");
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_revitFilesNeedUpdate)
            {
                foreach (RevitFile revitFile in BrowserFiles)
                {
                    revitFile.Status = FileStatus.GetVersion; ;
                    await SetRevitVersion(revitFile);
                    revitFile.Status = FileStatus.ReadyToUpload;
                }

                StateHasChanged();

                _revitFilesNeedUpdate = false;
            }

        }

        public async Task SetRevitVersion(RevitFile revitFile)
        {
            if (!string.IsNullOrEmpty(revitFile.Version)) return;

            long maxFileSize = 1024 * 1024 * 600; // 600 MB

            string path = Path.GetTempFileName();
            await using FileStream fs = new(path, FileMode.Create);
            await revitFile.OpenReadStream(maxFileSize).CopyToAsync(fs);

            StringBuilder result = new StringBuilder();

            CompoundFile cf = new CompoundFile(fs);
            CFStream foundStream = cf.RootStorage.GetStream("BasicFileInfo");
            byte[] foundStreamData = foundStream.GetData();

            string fileInfoAsText = UTF8Encoding.Unicode.GetString(foundStreamData);
            string[] basicFileInfoParts = fileInfoAsText.Split(new char[] { '\0' });

            cf.Close();

            int[] partIndexes = { 5, 7 };

            foreach (int partIndex in partIndexes)
            {
                Match match = Regex.Match(basicFileInfoParts[partIndex], "(\\d{4})");
                if (match.Success)
                {
                    revitFile.Version = match.Groups[0].Value;
                    break;
                }
            }

            return;

        }


    }

    static class Constants
    {
        public const int MaxRetry = 5;
        public const long ChunkSize = 5 * 1024 * 1024;
        public const int BatchSize = 25;
    }
}
