
using Application;
using Infrastructure;
using Microsoft.Identity.Web;
using Stripe;
using Azure.Security.KeyVault;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Extensions.Azure;

namespace WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Services.AddIdentityServices();

            string userAssignedClientId = builder.Configuration["ManagedIdentityClientId"];


            // Add services to the container.
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDevAzureServices(builder);
            }
            else
            {
                builder.Services.AddAzureServices(builder);
            }
            
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdB2C");


            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }

            if (app.Environment.IsDevelopment())
            {
                // Initialise and seed database
                using (IServiceScope scope = app.Services.CreateScope())
                {
                    DbContextInitialiser ApPInitialiser = scope.ServiceProvider.GetRequiredService<DbContextInitialiser>();
                    await ApPInitialiser.InitialiseAsync();
                    await ApPInitialiser.SeedAsync();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseBlazorFrameworkFiles();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }

    public static class ConfigureAzureServices
    {
        public static IServiceCollection AddDevAzureServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddTableServiceClient(builder.Configuration.GetSection("Storage"));
                clientBuilder.UseCredential(new DefaultAzureCredential());
            });

            return services;
        }
        public static IServiceCollection AddAzureServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            string storageAccountName = builder.Configuration["StorageAccountName"];
            string tableEndpoint = $"https://{storageAccountName}.table.core.windows.net/";
            string userAssignedClientId = builder.Configuration["ManagedIdentityClientId"];
            string keyVaultEndPoint = $"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/";

            DefaultAzureCredential credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = userAssignedClientId
            });

            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultEndPoint), credential);

            services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddTableServiceClient(new Uri(tableEndpoint));

                clientBuilder.UseCredential(credential);
            });

            return services;
        }
    }
}