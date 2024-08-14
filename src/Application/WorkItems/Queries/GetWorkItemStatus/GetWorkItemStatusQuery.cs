using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

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
