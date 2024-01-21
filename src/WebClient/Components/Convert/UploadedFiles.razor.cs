using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using OpenMcdf;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Text;
using WebClient.Models;

namespace WebClient.Components.Convert
{
    public partial class UploadedFiles
    {
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

        private void UploadFile()
        {
            // Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("TODO: Upload your files!");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_revitFilesNeedUpdate)
            {
                foreach (RevitFile revitFile in BrowserFiles)
                {
                    await SetRevitVersion(revitFile);
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
}
