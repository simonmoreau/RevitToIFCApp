using Microsoft.AspNetCore.Components.Forms;
using System.Collections.ObjectModel;
using WebClient.Models;

namespace WebClient.Components.Convert
{
    public partial class Upload
    {

        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string _dragClass = DefaultDragClass;
        private readonly ObservableCollection<RevitFile> _revitFiles = new();

        private async Task Clear()
        {
            _revitFiles.Clear();
            ClearDragClass();
            await Task.Delay(100);
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

        private void AddFile()
        {

        }

        private void SetDragClass()
            => _dragClass = $"{DefaultDragClass} mud-border-primary";

        private void ClearDragClass()
            => _dragClass = DefaultDragClass;
    }
}


