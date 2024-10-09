using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WebClient.Services
{

    public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation)
            : base(provider, navigation)
        {
            
            ConfigureHandler(
                authorizedUrls: new[] { "https://app-bim42-prod-fr-rvt2ifc.azurewebsites.net/" },
                scopes: new[] { "https://revittoifc.onmicrosoft.com/6cf133bd-e341-4a84-9985-e5f03811f7d3/API.Access" });
        }
    }
}




