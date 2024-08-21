using Application.WorkItems.Queries.GetWorkItem;
using Domain.Entities;
using MediatR;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, string>
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GetAllUsersQueryHandler(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<string> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            Microsoft.Graph.Models.UserCollectionResponse? result = await _graphServiceClient.Users.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "displayName", "id" };
            });

            return result.Value.First().DisplayName;
        }
    }
}
