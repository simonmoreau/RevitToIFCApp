using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using WebClient.Models;

namespace WebClient.Components.Convert
{
    public partial class UploadedFiles
    {
        [Parameter]
        public List<RevitFile> BrowserFiles { get; set; } = default!;

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
    }
}
