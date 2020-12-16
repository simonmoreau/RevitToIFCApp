using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Autodesk.Forge.Core;
using Autodesk.Forge;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

[assembly: FunctionsStartup(typeof(api.Startup))]
namespace api
{
  public class Startup : FunctionsStartup
  {
    public override void Configure(IFunctionsHostBuilder builder)
    {
      IConfigurationBuilder configBuilder = new ConfigurationBuilder();

      configBuilder = ForgeAlternativeConfigurationExtensions.AddForgeAlternativeEnvironmentVariables(configBuilder);
      configBuilder.Build();

      IConfiguration configuration = configBuilder.Build(); // builder.GetContext().Configuration;

      builder.Services.AddDesignAutomation(configuration);
      builder.Services.AddSingleton<Autodesk.Forge.TwoLeggedApi>(new TwoLeggedApi());

      // Initialize the client credential auth provider
      IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
          .Create(Environment.GetEnvironmentVariable("AppId"))
          .WithTenantId(Environment.GetEnvironmentVariable("TenantId"))
          .WithClientSecret(Environment.GetEnvironmentVariable("ClientSecret"))
          .Build();
      ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);
      GraphServiceClient graphServiceClient = new GraphServiceClient(authProvider);
      // Set up the Microsoft Graph service client with client credentials
      builder.Services.AddSingleton<GraphServiceClient>(graphServiceClient);

      builder.Services.AddSingleton<Utilities>(new Utilities(graphServiceClient));
    }
  }
}