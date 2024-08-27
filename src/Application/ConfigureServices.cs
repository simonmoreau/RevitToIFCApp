using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Azure;
using Autodesk.Oss;
using Autodesk.SDKManager;
using Autodesk.Authentication;
using Domain.Entities;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;


namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddLogging();

        SDKManager sdkManager = SdkManagerBuilder.Create().Build();
        AuthenticationClient authClient = new AuthenticationClient(sdkManager);

        services.AddSingleton<AuthenticationClient>(authClient);
        services.AddSingleton<SDKManager>(sdkManager);
        services.AddDesignAutomation(configuration);
        services.AddOss(configuration);
        services.AddTransient<OssClient>();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(configuration["Azure:ConnectionString"]);
        });

        services.Configure<ForgeConfiguration>(configuration.GetSection("Forge"));
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        services.Configure<AzureB2CSettings>(configuration.GetSection("AzureAdB2C"));


        AzureB2CSettings? azureB2CSettings = configuration.GetSection("AzureAdB2C").Get<AzureB2CSettings>();

        // Initialize the client credential auth provider
        string[] scopes = new[] { "https://graph.microsoft.com/.default" };

        ClientSecretCredential clientSecretCredential = new ClientSecretCredential(azureB2CSettings.TenantId, azureB2CSettings.ClientId, azureB2CSettings.ClientSecret);

        //you can use a single client instance for the lifetime of the application
        services.AddSingleton<GraphServiceClient>(sp => {
            return new GraphServiceClient(clientSecretCredential, scopes);
        });

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
