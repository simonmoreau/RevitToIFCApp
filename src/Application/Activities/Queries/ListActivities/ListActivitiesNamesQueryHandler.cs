using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Azure.Data.Tables;
using MediatR;

namespace Application.Activities.Queries.ListActivities
{

    public class ListActivitiesNamesQueryHandler : IRequestHandler<ListActivitiesNamesQuery, List<string>>
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly IActivitiesApi _activitiesApi;

        public ListActivitiesNamesQueryHandler(TableServiceClient tableServiceClient, IActivitiesApi activitiesApi)
        {
            _tableServiceClient = tableServiceClient;
            _activitiesApi = activitiesApi;
        }

        public async Task<List<string>> Handle(ListActivitiesNamesQuery request, CancellationToken cancellationToken)
        {
            Autodesk.Forge.Core.ApiResponse<Page<string>> activities = await _activitiesApi.GetActivitiesAsync();

            return activities.Content.Data;
        }
    }
}
