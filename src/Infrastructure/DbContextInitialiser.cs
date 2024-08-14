using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure
{
    public class DbContextInitialiser
    {
        private readonly ILogger<DbContextInitialiser> _logger;
        private readonly AppDbContext _context;

        public DbContextInitialiser(ILogger<DbContextInitialiser> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            List<Objet> objets = new List<Objet>
                {
                    new Objet { Id = "ZjMxODljNzMtMjIxNC00OWM1LWI4YzEtNGFlNGVhMmNjZDQw", Name = "Reprise de l'enrobé", Station_Service = "Aire de la Chaponne", Travaux = DateTime.Now  },
                    new Objet { Id = "YTIxMzMyZjItYmZhZi00MDI3LTg0MmQtOWJkNDM5ODY5YWI5", Name = "Changement de cuve", Station_Service = "Aire du Haut-Koenigsbourg", Travaux = DateTime.Now,  }
                };

            if (!_context.Objets.Any())
            {
                _context.Objets.AddRange(objets);
            }

            await _context.SaveChangesAsync();
        }
    }
}