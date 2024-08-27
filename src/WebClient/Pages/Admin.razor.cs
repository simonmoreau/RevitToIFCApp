using Microsoft.AspNetCore.Components;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Pages
{
    public partial class Admin
    {
        [Inject]
        public IDataService _dataService { get; set; }

        ListForgeApplicationsVm ListForgeApplicationsVm { get; set; }
        UserDTO User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ListForgeApplicationsVm = await _dataService.GetApplicationDetails();
            User = await _dataService.GetMe();

        }

    }
}
