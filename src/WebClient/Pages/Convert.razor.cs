using Microsoft.AspNetCore.Components;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Pages
{
    public partial class Convert
    {
        [Inject]
        public IDataService _dataService { get; set; }

        UserDTO User { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (User == null)
            {
                User = await _dataService.GetMe();
            }
            

        }
    }
}
