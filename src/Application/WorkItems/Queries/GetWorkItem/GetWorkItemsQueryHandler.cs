using Application.Common.Interfaces;
using Application.Common.Services;
using Autodesk.Forge.DesignAutomation.Http;
using Azure.Data.Tables;
using Domain.Entities;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem
{
    public class GetWorkItemsQueryHandler : IRequestHandler<GetWorkItemsQuery, List<SavedWorkItem>>
    {
        private readonly IAppDbContext _context;
        private readonly ISavedWorkItemService _savedWorkItemService;

        public GetWorkItemsQueryHandler(IAppDbContext context, ISavedWorkItemService savedWorkItemService)
        {
            _context = context;
            _savedWorkItemService = savedWorkItemService;
        }

        public async Task<List<SavedWorkItem>> Handle(GetWorkItemsQuery request, CancellationToken cancellationToken)
        {
            List<SavedWorkItem> savedWorkItems = _savedWorkItemService.GetSavedWorkItems(request.UserId);

            return await Task.FromResult<List<SavedWorkItem>>(savedWorkItems);
        }
    }
}
