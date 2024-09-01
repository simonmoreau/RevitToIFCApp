using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<Objet> Objets { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
