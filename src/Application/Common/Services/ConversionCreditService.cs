using Application.Common.Exceptions;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Application.Common.Services
{
    internal class ConversionCreditService : IConversionCreditService
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly AzureB2CSettings _azureB2CSettings;
        private readonly string _conversionCreditsAttributeName;

        public ConversionCreditService(GraphServiceClient graphServiceClient, IOptions<AzureB2CSettings> azureB2CSettings)
        {
            _graphServiceClient = graphServiceClient;
            _azureB2CSettings = azureB2CSettings.Value;
            _conversionCreditsAttributeName = AdditionalParameterRetriever.GetConversionCreditsAttributeName(_azureB2CSettings.B2cExtensionAppClientId);
        }

        public async Task<int> EditConversionCredits(string userId, int quantity)
        {
            User? user = await GetUser(userId);

            if (user == null)
            {
                throw new NotFoundException($"The user {userId} was not found.", userId);
            }

            decimal credits = 0;
            if (user.AdditionalData.ContainsKey(_conversionCreditsAttributeName))
            {
                credits = (decimal)user.AdditionalData[_conversionCreditsAttributeName];
            }

            credits = credits + quantity;

            user.AdditionalData[_conversionCreditsAttributeName] = credits;

            var requestBody = new User
            {
                AdditionalData = new Dictionary<string, object>
                {
                    {
                        _conversionCreditsAttributeName , credits.ToString()
                    }
                },
            };

            User? updatedUser = await _graphServiceClient.Users[user.Id].PatchAsync(requestBody);

            if (updatedUser == null)
            {
                updatedUser = await GetUser(userId);
            }

            return decimal.ToInt16((decimal)updatedUser.AdditionalData[_conversionCreditsAttributeName]);
        }

        public async Task<int> GetConversionCredits(string userId)
        {
            User? user = await GetUser(userId);

            if (user == null)
            {
                throw new NotFoundException($"The user {userId} was not found.", userId);
            }

            decimal credits = 0;
            if (user.AdditionalData.ContainsKey(_conversionCreditsAttributeName))
            {
                credits = (decimal)user.AdditionalData[_conversionCreditsAttributeName];
            }

            return decimal.ToInt16(credits);
        }

        public async Task<User?> GetUser(string userId)
        {
            string selects = $"displayName,id,mail,mobilePhone,{_conversionCreditsAttributeName}";

            User? user = await _graphServiceClient.Users[userId].GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "Id", "displayName", _conversionCreditsAttributeName };
            });

            return user;
        }
    }
}
