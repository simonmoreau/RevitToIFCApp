using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities.Queries.ListActivities
{
    public class ListActivitiesNamesQuery : IRequest<List<string>>
    {
    }
}
