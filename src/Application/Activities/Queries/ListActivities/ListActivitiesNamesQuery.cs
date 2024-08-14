using MediatR;

namespace Application.Activities.Queries.ListActivities
{
    public class ListActivitiesNamesQuery : IRequest<List<string>>
    {
    }
}
