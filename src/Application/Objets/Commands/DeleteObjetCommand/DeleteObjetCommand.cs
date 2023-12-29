using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objets.Commands.DeleteObjetCommand
{
    public class DeleteObjetCommand : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteObjetCommand>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context)
            {
                _context = context;
            }

            public async Task Handle(DeleteObjetCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Objets
                    .FindAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException("Introuvable", request.Id);
                }

                _context.Objets.Remove(entity);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
