using Application.Common.Exceptions;
using Application.WorkItems.Queries.GetWorkItem;
using Domain.Entities;
using MediatR;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, string>
    {
        private readonly GraphServiceClient _graphServiceClient;

        public GetUserQueryHandler(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<string> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            //var meUser = await _graphServiceClient.Me.GetAsync();

            Microsoft.Graph.Models.User? user = await _graphServiceClient.Users[request.UserId].GetAsync();

            if (user == null)
            {
                throw new NotFoundException($"The user {request.UserId} was not found.", request.UserId);
            }

            return user.DisplayName;
        }
    }
}
