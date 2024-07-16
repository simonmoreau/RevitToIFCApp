
using Microsoft.AspNetCore.Components;
using WebClient.Models;
using WebClient.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebClient.Pages
{
    public partial class Checkout
    {
        [Inject]
        public IDataService _dataService { get; set; }

        ListForgeApplicationsVm ListForgeApplicationsVm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ListForgeApplicationsVm = await _dataService.GetApplicationDetails();
        }

    }
}
