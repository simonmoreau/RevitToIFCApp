using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace WebClient.Services
{

    public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, IConfiguration configuration, IWebAssemblyHostEnvironment webAssemblyHostEnvironment)
            : base(provider, navigation)
        {

            string apiUrl = webAssemblyHostEnvironment.BaseAddress;

            if (!webAssemblyHostEnvironment.IsDevelopment())
            {
                apiUrl = configuration.GetValue<string>("APIAdress");
            }

            ConfigureHandler(
                authorizedUrls: new[] { apiUrl },
                scopes: new[] { "https://revittoifc.onmicrosoft.com/6cf133bd-e341-4a84-9985-e5f03811f7d3/API.Access" });
        }
    }
}




