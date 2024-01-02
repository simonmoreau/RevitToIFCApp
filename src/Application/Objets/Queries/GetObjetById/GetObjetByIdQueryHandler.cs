using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objets.Queries.GetObjetById
{
    public class GetObjetByIdQueryHandler
    {
        private readonly IAppDbContext _context;

        public GetObjetByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Objet> Handle(GetObjetByIdQuery request, CancellationToken cancellationToken)
        {
            Objet? objet = await _context.Objets
                .Where(v => v.Id == request.Id)
                .FirstOrDefaultAsync();

            if (objet == null)
            {
                throw new NotFoundException("Objet introuvable", request.Id);
            }
            else
            {
                ObjetByIdVm vm = new ObjetByIdVm
                {
                    Objet = objet
                };

                return vm.Objet;
            }
        }
    }
}
