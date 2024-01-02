using Application.Common.Interfaces;
using Application.Sites.Queries.GetSiteList;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sites.Queries.GetSiteList
{
    public class GetSiteListQueryHandler : IRequestHandler<GetSiteListQuery, SiteListVm>
    {
        private readonly IAppDbContext _context;

        public GetSiteListQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<SiteListVm> Handle(GetSiteListQuery request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Site> sites = await _context.Sites
                .OrderBy(v => v.Name)
                .ToListAsync(cancellationToken);

            SiteListVm vm = new SiteListVm
            {
                Sites = sites
            };

            return vm;
        }
    }
}
