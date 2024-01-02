using Application.Common.Interfaces;
using Application.Objets.Queries.GetObjetList;
using MediatR;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Objets.Queries.GetPermisList
{
    public class GetPlanListQueryHandler : IRequestHandler<GetPlanListQuery, PlanListVm>
    {
        private readonly IAppDbContext _context;

        public GetPlanListQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<PlanListVm> Handle(GetPlanListQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Plan> plan = await _context.Plans
                .OrderBy(v => v.Name)
                .ToListAsync(cancellationToken);

            PlanListVm vm = new PlanListVm
            {
                Plans = plan
            };

            return vm;
        }
    }
}
