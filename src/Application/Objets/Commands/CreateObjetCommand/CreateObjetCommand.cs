using Application.Common.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;

namespace Application.Objets.Commands.CreateObjetCommand
{
    public class CreateObjetCommand : IRequest<string>
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Station_Service { get; set; }
        public DateTime? Travaux { get; set; }



        public class Handler : IRequestHandler<CreateObjetCommand, string>
        {
            private readonly IAppDbContext _context;

            public Handler(IAppDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(CreateObjetCommand request, CancellationToken cancellationToken)
            {
                Objet entity = new Objet
                {
                    Id = IdCreator.RandomString(),
                    Name = request.Name,
                    Station_Service = request.Station_Service,
                    Travaux = request.Travaux
                };
                _context.Objets.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;

            }
        }
    }
}
