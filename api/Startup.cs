using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Autodesk.Forge.Core;

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
        }
    }
}