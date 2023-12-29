using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("CleanArchitectureDb"));

        services.AddScoped((Func<IServiceProvider, IAppDbContext>)(provider => provider.GetRequiredService<Infrastructure.AppDbContext>()));

        services.AddScoped<DbContextInitialiser>();

        return services;
    }
}
