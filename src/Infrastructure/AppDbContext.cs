using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using MediatR;
using Domain.Entities;

namespace Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext():base()
        {

        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }
        public DbSet<Objet> Objets { get; set; }
        public DbSet<Company> Companies { get ; set ; }
        public DbSet<Site> Sites { get ; set ; }
        public DbSet<ConstructionSite> ConstructionSites { get; set; }
        public DbSet<Plan> Plans { get ; set ; }
        public DbSet<Question> Questions { get ; set ; }
        public DbSet<QuestionTemplate> QuestionTemplates { get ; set ; }
        public DbSet<Answer> Answers { get; set; }

        public DbSet<User> Users { get ; set ; }
        Task<int> IAppDbContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        //{
        //    string connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiAppData";
        //    dbContextOptionsBuilder
        //        .UseSqlServer(connectionString);
        //    base.OnConfiguring(dbContextOptionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Objet>()
                .ToTable("OBJETS");

            modelBuilder.Entity<Company>()
                .ToTable("COMPANIES");
        }
    }
}
