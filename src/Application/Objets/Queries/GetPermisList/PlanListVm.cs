using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Objets.Queries.GetPermisList
{
    public class PlanListVm
    {
        public IList<Plan>? Plans { get; set; }
    }
}
