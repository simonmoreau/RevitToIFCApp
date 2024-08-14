using Application.Common.Interfaces;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Azure.Data.Tables;
using MediatR;

namespace Application.Activities.Queries.GetActivity
{
    public class GetActivityQueryHandler : IRequestHandler<GetActivityQuery, Activity>
    {
        private readonly IAppDbContext _context;
        private readonly TableServiceClient _tableServiceClient;
        private readonly IActivitiesApi _activitiesApi;

        public GetActivityQueryHandler(IAppDbContext context, TableServiceClient tableServiceClient, IActivitiesApi activitiesApi)
        {
            _context = context;
            _tableServiceClient = tableServiceClient;
            _activitiesApi = activitiesApi;
        }

        public async Task<Activity> Handle(GetActivityQuery request, CancellationToken cancellationToken)
        {
            Autodesk.Forge.Core.ApiResponse<Activity> apiResponse = await _activitiesApi.GetActivityAsync(request.Id);

            return apiResponse.Content;
        }
    }
}
