using Domain.Entities;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem
{
    public class GetWorkItemsQuery : IRequest<List<SavedWorkItem>>
    {
        internal readonly string UserId;

        public GetWorkItemsQuery(string userId)
        {
            UserId = userId;
        }
    }
}
