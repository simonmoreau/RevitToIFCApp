using Application.Common.Exceptions;
using Application.Common.Services;
using Application.WorkItems.Queries.GetWorkItem;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly AzureB2CSettings _azureB2CSettings;

        public GetUserQueryHandler(GraphServiceClient graphServiceClient, IOptions<AzureB2CSettings> azureB2CSettings)
        {
            _graphServiceClient = graphServiceClient;
            _azureB2CSettings = azureB2CSettings.Value;
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
           string conversionCreditsAttributeName = AdditionalParameterRetriever.GetConversionCreditsAttributeName(_azureB2CSettings.B2cExtensionAppClientId);

           string selects = $"displayName,id,mail,mobilePhone,{conversionCreditsAttributeName}";

            Microsoft.Graph.Models.User? user = await _graphServiceClient.Users[request.UserId].GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] {"Id", "displayName",conversionCreditsAttributeName };
            });

            if (user == null)
            {
                throw new NotFoundException($"The user {request.UserId} was not found.", request.UserId);
            }

            decimal credis = 0;
            if (user.AdditionalData.ContainsKey(conversionCreditsAttributeName))
            {
                credis = (decimal)user.AdditionalData[conversionCreditsAttributeName];
            }

            UserDTO userDTO = new UserDTO()
            {
                Id = user.Id,
                Name = user.DisplayName,
                ConversionCredits = decimal.ToInt16(credis)
            }; 
            return userDTO;
        }


    }
}
