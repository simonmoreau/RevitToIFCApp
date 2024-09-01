using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace ApplicationTest.Common
{
    public class AppContextFactory
    {
        public static AppDbContext Create()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            AppDbContext context = new AppDbContext(options);
            context.Database.EnsureCreated();

            DbContextInitialiser dbContextInitialiser = new DbContextInitialiser(NullLogger<DbContextInitialiser>.Instance, context);
            dbContextInitialiser.InitialiseAsync().Wait();
            dbContextInitialiser.SeedAsync().Wait();

            return context;
        }

        public static void Destroy(AppDbContext context)
        {
            context.Database.EnsureDeleted();

            context.Dispose();
        }
    }
}
