using Azure;
using Azure.Data.Tables;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly TableServiceClient _tableServiceClient;
        private const string _partitionKey = "conversionCheckoutSession";

        public CheckoutService(TableServiceClient tableServiceClient)
        {
            _tableServiceClient = tableServiceClient;
        }

        public async Task<ConversionCheckoutSession?> GetCheckoutSession(string checkoutSessionId)
        {
            TableClient tableClient = _tableServiceClient.GetTableClient(_partitionKey);
            tableClient.CreateIfNotExists();

             NullableResponse <ConversionCheckoutSession> response = await tableClient.GetEntityIfExistsAsync<ConversionCheckoutSession>(_partitionKey, checkoutSessionId);

            if (!response.HasValue)
            {
                return null;
            }
            else
            {
                return response.Value;
            }
        }

        public async Task<ConversionCheckoutSession> CreateCheckoutSession(string sessionId)
        {
            ConversionCheckoutSession conversionCheckoutSession = new ConversionCheckoutSession();
            conversionCheckoutSession.PartitionKey = _partitionKey;
            conversionCheckoutSession.RowKey = sessionId;
            conversionCheckoutSession.UserId = null;
            conversionCheckoutSession.SessionId = sessionId;

            TableClient tableClient = _tableServiceClient.GetTableClient(_partitionKey);
            tableClient.CreateIfNotExists();

            Azure.Response response = await tableClient.AddEntityAsync<ConversionCheckoutSession>(conversionCheckoutSession);

            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return conversionCheckoutSession;
        }

        public async Task UpdateCheckoutSessionStatus(string sessionId, ConversionCheckoutStatus conversionCheckoutStatus)
        {
            TableClient tableClient = _tableServiceClient.GetTableClient(_partitionKey);
            tableClient.CreateIfNotExists();

            ConversionCheckoutSession conversionCheckoutSession = await tableClient.GetEntityAsync<ConversionCheckoutSession>(_partitionKey, sessionId);
            conversionCheckoutSession.Status = conversionCheckoutStatus;

            Azure.Response response = await tableClient.UpdateEntityAsync(conversionCheckoutSession, Azure.ETag.All);

            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
