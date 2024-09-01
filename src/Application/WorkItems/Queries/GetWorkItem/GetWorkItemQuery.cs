using Domain.Entities;
using MediatR;

namespace Application.WorkItems.Queries.GetWorkItem
{
    public class GetWorkItemQuery : IRequest<SavedWorkItem>
    {
    }
}
