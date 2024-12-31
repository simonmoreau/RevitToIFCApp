using Microsoft.AspNetCore.Components;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Pages
{
    public partial class CheckoutSuccess
    {
        [Inject]
        public IDataService _dataService { get; set; }

        [Parameter]
        public string? SessionId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (SessionId != null)
            {
                await _dataService.FullfilCheckoutSession(SessionId);
            }
        }
    }
}
