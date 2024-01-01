using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace WebClient.Components.Convert
{
    public partial class Upload
    {

        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string _dragClass = DefaultDragClass;
        private readonly List<IBrowserFile> _browserFiles = new();

        private async Task Clear()
        {
            _browserFiles.Clear();
            ClearDragClass();
            await Task.Delay(100);
        }

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                _browserFiles.Add(file);
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


