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
    }
}
