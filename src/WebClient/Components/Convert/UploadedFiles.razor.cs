using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace WebClient.Components.Convert
{
    public partial class UploadedFiles
    {
        [Parameter]
        public List<IBrowserFile> BrowserFiles { get; set; } = default!;

        private void RemoveFile(IBrowserFile file)
        {
            BrowserFiles.Remove(file);
        }

        private void UploadFile()
        {
            // Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("TODO: Upload your files!");
        }
    }
}
