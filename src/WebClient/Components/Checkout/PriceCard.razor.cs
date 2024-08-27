using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using WebClient.Models;
using WebClient.Services;


namespace WebClient.Components.Checkout
{
    public partial class PriceCard
    {
        [Inject]
        private IDataService _dataService { get; set; }

        [Inject]
        private NavigationManager _navManager { get; set; }

        [Parameter]
        public string Title { get; set; } = "";

        [Parameter]
        public string SubTitle { get; set; } = "";

        [Parameter]
        public string Description { get; set; } = "";

        [Parameter]
        public string PriceId { get; set; } = "";

        [CascadingParameter] protected Task<AuthenticationState> AuthStat { get; set; }
        private async void GotToCheckoutSession()
        {
            var user = (await AuthStat).User;

            if (!user.Identity.IsAuthenticated)
            {
                _navManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(_navManager.Uri)}");
            }
            else
            {
                CheckoutSessionDTO checkoutSession = await _dataService.GetCheckoutSession(PriceId);
                string checkoutSessionUrl = checkoutSession.Url;

                _navManager.NavigateTo(checkoutSessionUrl);
            }

        }
    }
}
