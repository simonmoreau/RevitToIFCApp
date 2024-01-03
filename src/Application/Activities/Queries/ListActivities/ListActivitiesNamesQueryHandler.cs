using Application.Activities.Queries.GetActivity;
using Application.Common.Interfaces;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Azure.Data.Tables;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities.Queries.ListActivities
{

    public class ListActivitiesNamesQueryHandler : IRequestHandler<ListActivitiesNamesQuery, List<string>>
    {
        private readonly IAppDbContext _context;
        private readonly TableServiceClient _tableServiceClient;
        private readonly IActivitiesApi _activitiesApi;

        public ListActivitiesNamesQueryHandler(IAppDbContext context, TableServiceClient tableServiceClient, IActivitiesApi activitiesApi)
        {
            _context = context;
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
