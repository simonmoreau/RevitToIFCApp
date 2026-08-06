using Application.Common.Services;
using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItemStatus
{
    public class GetWorkItemStatusQueryHandler : IRequestHandler<GetWorkItemStatusQuery, WorkItemStatus>
    {
        private readonly ISavedWorkItemService _savedWorkItemService;

        public GetWorkItemStatusQueryHandler(ISavedWorkItemService savedWorkItemService)
        {
            _savedWorkItemService = savedWorkItemService;
        }

        public async Task<WorkItemStatus> Handle(GetWorkItemStatusQuery request, CancellationToken cancellationToken)
        {
            string id = request.WorkItemId;

            WorkItemStatus workItemStatus = await _savedWorkItemService.GetSavedWorkItemStatus(id);

            return workItemStatus;
        }
    }
}
