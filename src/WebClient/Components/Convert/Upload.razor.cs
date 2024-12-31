using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.ObjectModel;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Components.Convert
{
    public partial class Upload
    {
        [Inject]
        public IDataService _dataService { get; set; }

        [Inject]
        public IUploadService _uploadService { get; set; }

        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string _dragClass = DefaultDragClass;
        public readonly ObservableCollection<RevitFile> RevitFiles = new();
        private MudFileUpload<IReadOnlyList<IBrowserFile>>? _fileUpload;

        private async Task ClearAsync()
        {
            await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);
            RevitFiles.Clear();
            ClearDragClass();
        }

        private Task OpenFilePickerAsync()
        {
            return _fileUpload?.OpenFilePickerAsync() ?? Task.CompletedTask;
        }

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            IReadOnlyList<IBrowserFile> files = e.GetMultipleFiles();
            foreach (IBrowserFile file in files)
            {
                RevitFile revitFile = new RevitFile(file);
                RevitFiles.Add(revitFile);
                revitFile.FileRemoved += RevitFileRemoved;
            }
        }

        private void RevitFileRemoved(object? sender, EventArgs e)
        {
            if (sender == null) return;
            RevitFile revitFile = (RevitFile)sender;
            if (revitFile == null) return;
            RevitFiles.Remove(revitFile);
            StateHasChanged();
        }

        private async Task UploadAll()
        {
            List<Task> uploadTasks =  new List<Task>();
            foreach (RevitFile revitFile in RevitFiles)
            {
                uploadTasks.Add(revitFile.UploadFile(_dataService, _uploadService));
            }

            await Task.WhenAll(uploadTasks);

            await InvokeAsync(StateHasChanged);
        }

        private void SetDragClass()
        {
            _dragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            _dragClass = DefaultDragClass;
        }
    }
}


