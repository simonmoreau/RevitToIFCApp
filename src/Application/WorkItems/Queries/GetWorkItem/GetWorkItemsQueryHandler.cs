using Application.Common.Services;
using Domain.Entities;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem
{
    public class GetWorkItemsQueryHandler : IRequestHandler<GetWorkItemsQuery, List<SavedWorkItem>>
    {
        private readonly ISavedWorkItemService _savedWorkItemService;

        public GetWorkItemsQueryHandler(ISavedWorkItemService savedWorkItemService)
        {
            _savedWorkItemService = savedWorkItemService;
        }

        public async Task<List<SavedWorkItem>> Handle(GetWorkItemsQuery request, CancellationToken cancellationToken)
        {
            List<SavedWorkItem> savedWorkItems = _savedWorkItemService.GetSavedWorkItems(request.UserId);

            return await Task.FromResult<List<SavedWorkItem>>(savedWorkItems);
        }
    }
}
