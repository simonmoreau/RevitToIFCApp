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

            List<Company> companies = new List<Company>
                {
                    new Company("f9ff86eb4f95403c94ae6deeb28b41a8") { Name = "StationTop1Monde"},
                    new Company("8e9a9ae76ccd4e08ad1d8b501e1d9067") { Name = "StationTop2Monde"}
                };

            if (!_context.Companies.Any())
            {
                _context.Companies.AddRange(companies);
            }

            List<Site> sites = new List<Site>
                {
                    new Site("d1f4d7de745140e09f5bc2b6338ab057", companies[0])
                    {
                        Name = "Site le plus beau",
                        Localisation = "Paris"
                    },
                    new Site("4744505a588b42ae8e3aa47971646960", companies[0])
                    {
                        Name = "Site le moins beau",
                        Localisation = "Berlin"
                    },
                    new Site("11c89f1095a143a1b94d21343534ddb5", companies[1])
                    {
                        Name = "Site moyen beau",
                        Localisation = "Londres"
                    },
                    new Site("a52bf03f4fb0421ba3111f741c14b9e1", companies[1])
                    {
                        Name = "Site plutot beau",
                        Localisation = "Rome"
                    },
                };

            if (!_context.Sites.Any())
            {
                _context.Sites.AddRange(sites);
            }

            List<ConstructionSite> constructionSites = new List<ConstructionSite>
                {
                    new ConstructionSite("9ac618887f954855b562c8f42255419d", sites[0])
                    {
                        Name = "Station Montmartre deuxième pompe",
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(7)
                    },
                    new ConstructionSite("3e649abd3b16490385c0bc93f495c998", sites[0])
                    {
                        Name = "Station effeil rénovation tuyau",
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(4)
                    },
                    new ConstructionSite("d9a333a076674e7bb07c04dfaaa5f8ca", sites[0])
                    {
                        Name = "Station Notre Dame pompe vandalisée",
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(6)
                    },
                    new ConstructionSite("acef98dc3637489092c360a9acec4501", sites[1])
                    {
                        Name = "La station big ben fuit",
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(15)
                    },
                };

            if (_context.ConstructionSites.Any())
            {
                _context.ConstructionSites.AddRange(constructionSites);
            }

            List<Plan> plans = new List<Plan>
                {
                    new Plan("d8fac85acf454b13a0900a1959d6a28e", constructionSites[0])
                    {
                        Name = "Entreprise blbl",
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(5),
                        Infos = "doit tout réparer"
                    },
                    new Plan("ae937387c5b647b6b4a64e6520de2034", constructionSites[3])
                    {
                        Name = "Entreprise babebibobu",
                        DateStart = DateTime.Now,
                        DateEnd = DateTime.Now.AddDays(4),
                        Infos = "peut prendre sa pause quand elle veut"
                    },
                };

            if (_context.Plans.Any())
            {
                _context.Plans.AddRange(plans);
            }

            List<QuestionTemplate> questionTemplates = new List<QuestionTemplate>
                {
                    new QuestionTemplate("f9ff86eb4f95403c94ae6deeb28b41a8")
                    {
                        Text = "La première question est géniale",
                        Groupe = "groupe1",
                        RoleResponsible = Domain.Enums.Role.All,
                        Type = Domain.Enums.Type.QuesionOuiNon
                    },
                    new QuestionTemplate("22c7401b65de4987a58399f831f79629")
                    {
                        Text = "La deuxième question est moins bien",
                        Groupe = "groupe2",
                        RoleResponsible = Domain.Enums.Role.Intervenant,
                        Type = Domain.Enums.Type.QuestionMesure
                    },
                    new QuestionTemplate("18a03ac9a1284310adf6dc72afb8c064")
                    {
                        Text = "La troisième question est franchement pas mal",
                        Groupe = "groupe1",
                        RoleResponsible = Domain.Enums.Role.Exploitant,
                        Type = Domain.Enums.Type.QuesionOuiNon
                    },
                    new QuestionTemplate("68711a1a5a304e25ac5c44bba9d88a79")
                    {
                        Text = "La quatrième question est mouais",
                        Groupe = "groupe2",
                        RoleResponsible = Domain.Enums.Role.All,
                        Type = Domain.Enums.Type.QuestionObservation
                    },
                };

            if (!_context.QuestionTemplates.Any())
            {
                _context.QuestionTemplates.AddRange(questionTemplates);
            }

            await _context.SaveChangesAsync();
        }
    }
}