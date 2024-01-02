using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Objets.Queries.GetObjetList
{
    public class GetObjetListQueryHandler : IRequestHandler<GetObjetListQuery, ObjetListVm>
    {
        private readonly IAppDbContext _context;

        public GetObjetListQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ObjetListVm> Handle(GetObjetListQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Objet> objets = await _context.Objets
                .OrderBy(v => v.Name)
                .ToListAsync(cancellationToken);

            ObjetListVm vm = new ObjetListVm
            {
                Objets = objets
            };

            return vm;
        }
    }
}
