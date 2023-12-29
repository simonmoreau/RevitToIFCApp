using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objets.Commands.CreateObjetCommand
{
    public class CreatePlanCommand : IRequest<string>
    {
        public string IdPermis { get; set; }
        public ConstructionSite ConstructionSite { get; set; }
        public string Name { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Infos { get; set; }
        public class Handler : IRequestHandler<CreatePlanCommand, string>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
            {
                Plan entity = new Plan(request.IdPermis, request.ConstructionSite)
                {
                    Name = request.Name,
                    DateStart = (DateTime)request.DateStart,
                    DateEnd = (DateTime)request.DateEnd,
                    Infos = request.Infos,
                };
                _context.Plans.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;

            }
        }
    }
}
