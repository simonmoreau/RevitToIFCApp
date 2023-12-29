using Application.Common.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objets.Commands.CreateObjetCommand
{
    public class CreateConstructionSiteCommand : IRequest<string>
    {
        public string IdConstructionSite { get; set; }
        public Site Site { get; set; }
        public string? Name { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }

        public class Handler : IRequestHandler<CreateConstructionSiteCommand, string>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(CreateConstructionSiteCommand request, CancellationToken cancellationToken)
            {
                ConstructionSite entity = new ConstructionSite(request.IdConstructionSite, request.Site)
                {
                    Name = request.Name,
                    DateStart = (DateTime)request.DateStart,
                    DateEnd = (DateTime)request.DateEnd
                };
                _context.ConstructionSites.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;

            }
        }
    }
}
