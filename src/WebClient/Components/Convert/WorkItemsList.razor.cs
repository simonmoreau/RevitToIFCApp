using Microsoft.AspNetCore.Components;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Components.Convert
{
    public partial class WorkItemsList
    {
        [Inject]
        public IDataService _dataService { get; set; }

        List<SavedWorkItemDTO> SavedWorkItems { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            SavedWorkItems = await _dataService.GetSavedWorkItems();
        }

        protected override async Task OnParametersSetAsync()
        {
            List<Task> setDownloadUrl = new List<Task>();

            foreach (SavedWorkItemDTO savedWorkItem in SavedWorkItems)
            {
                setDownloadUrl.Add(DownloadUrl(savedWorkItem));
            }

            await Task.WhenAll(setDownloadUrl);
        }

        public async Task DownloadUrl(SavedWorkItemDTO savedWorkItem)
        {
            if (savedWorkItem.ObjectKey == null) return;
            if (savedWorkItem.FileName == null) return;

            Signeds3downloadResponse signedDownload = await _dataService.GetDownloadUrl(savedWorkItem.ObjectKey, savedWorkItem.FileName);

            savedWorkItem.DownloadUrl = signedDownload.Url;
        }
    }
}
