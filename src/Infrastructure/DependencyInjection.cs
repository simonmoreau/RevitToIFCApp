using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ePPJDB")));

#pragma warning disable CS8603 // Possible null reference return.
            services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());
#pragma warning restore CS8603 // Possible null reference return.

            return services;
        }
    }
}
