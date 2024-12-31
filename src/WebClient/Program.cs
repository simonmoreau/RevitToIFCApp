using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using WebClient;
using WebClient.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Uri baseAdresse = new Uri(builder.HostEnvironment.BaseAddress);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = baseAdresse });

Uri apiAdress = baseAdresse;

if (!builder.HostEnvironment.IsDevelopment())
{
    string url = builder.Configuration.GetValue<string>("APIAdress");
    apiAdress = new Uri(url);
}

builder.Services.AddTransient<ApiAuthorizationMessageHandler>();

builder.Services
    .AddHttpClient<IDataService, DataService>(client => client.BaseAddress = apiAdress)
    .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IUploadService, UploadService>(client => client.BaseAddress = apiAdress);

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://revittoifc.onmicrosoft.com/6cf133bd-e341-4a84-9985-e5f03811f7d3/API.Access");
    options.ProviderOptions.LoginMode = "redirect";
});


builder.Services.AddMudServices();


await builder.Build().RunAsync();