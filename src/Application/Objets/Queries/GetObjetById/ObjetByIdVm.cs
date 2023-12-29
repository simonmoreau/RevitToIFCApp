using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objets.Queries.GetObjetById
{
    public class ObjetByIdVm : IRequest<ObjetByIdVm>
    {
        public Objet Objet { get; set; }
    }
}
