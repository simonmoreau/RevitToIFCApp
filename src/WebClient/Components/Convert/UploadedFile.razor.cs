using Microsoft.AspNetCore.Components;
using MudBlazor;
using OpenMcdf;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Text;
using WebClient.Models;
using WebClient.Services;
using System.Reflection.Metadata;

namespace WebClient.Components.Convert
{
    public partial class UploadedFile
    {
        [Inject]
        public IDataService _dataService { get; set; }

        [Inject]
        public IUploadService _uploadService { get; set; }


        [Parameter]
        public RevitFile RevitFile { get; set; } = default!;

        protected override void OnInitialized()
        {
            //BrowserFiles.CollectionChanged += BrowserFiles_CollectionChanged;   
        }

        private void RemoveFile()
        {
            RevitFile.RaiseFileRemoved();
        }

        private async Task UploadFile()
        {
            await RevitFile.UploadFile(_dataService, _uploadService);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            RevitFile.Status = FileStatus.GetVersion;
            StateHasChanged();

            RevitFile.Version =  await GetRevitVersion(RevitFile);
            RevitFile.Status = FileStatus.ReadyToUpload;

            StateHasChanged();

            RevitFile.StatusChanged += RevitFile_StatusChanged;

        }

        private void RevitFile_StatusChanged(object? sender, EventArgs e)
        {
            StateHasChanged();
        }

        private async Task<string> GetRevitVersion(RevitFile revitFile)
        {
            if (!string.IsNullOrEmpty(revitFile.Version)) return revitFile.Version;

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
                    return match.Groups[0].Value;
                }
            }

            return "";

        }


    }
}
