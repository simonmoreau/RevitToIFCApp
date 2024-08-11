using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.WorkItems.Queries.GetWorkItemStatus
{
    public class GetWorkItemStatusQuery : IRequest<WorkItemStatus>
    {
        public readonly string WorkItemId;

        public GetWorkItemStatusQuery(string workItemId)
        {
            WorkItemId = workItemId;
        }
    }
}
