using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Autodesk.Forge.DesignAutomation;
using Microsoft.Extensions.Azure;
using Azure.Data.Tables;
using Autodesk.Oss;
using Autodesk.SDKManager;
using Microsoft.Extensions.Logging;
using Autodesk.Authentication;
using Domain.Entities;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddLogging();

        SDKManager sdkManager = SdkManagerBuilder.Create().Build();
        AuthenticationClient authClient = new AuthenticationClient(sdkManager);

        services.AddSingleton<AuthenticationClient>(authClient);
        services.AddSingleton(sdkManager);
        services.AddDesignAutomation(configuration);
        services.AddOss(configuration);

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddTableServiceClient(configuration["Azure:ConnectionString"]);
        });

        services.Configure<ForgeConfiguration>(configuration.GetSection("Forge"));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        });

        return services;
    }
}
