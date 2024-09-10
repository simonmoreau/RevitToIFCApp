using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Collections.ObjectModel;
using WebClient.Models;

namespace WebClient.Components.Convert
{
    public partial class Upload
    {
        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
        private string _dragClass = DefaultDragClass;
        private readonly ObservableCollection<RevitFile> _revitFiles = new();
        private MudFileUpload<IReadOnlyList<IBrowserFile>>? _fileUpload;

        private async Task ClearAsync()
        {
            await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);
            _revitFiles.Clear();
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
                _revitFiles.Add(revitFile);
            }
        }

        private void UploadAll()
        {
            foreach (RevitFile revitFile in _revitFiles)
            {
                
            }
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


