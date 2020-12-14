using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace api
{
    public static class UpdateConversionsToken
    {
        [FunctionName("UpdateConversionsToken")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed the UpdateConversionToken request.");

            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("stripe_api_key");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UpdateTokenBody updateTokenBody = JsonConvert.DeserializeObject<UpdateTokenBody>(requestBody);

            SessionService sessionService = new SessionService();
            Session session = sessionService.Get(updateTokenBody.sessionId);

            if (session != null)
            {
                // Get the product/price payed
                int tokens = Convert.ToInt32(Math.Floor(Convert.ToDecimal(session.AmountTotal) / 100));

                // Initialize the client credential auth provider
                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(Environment.GetEnvironmentVariable("AppId"))
                    .WithTenantId(Environment.GetEnvironmentVariable("TenantId"))
                    .WithClientSecret(Environment.GetEnvironmentVariable("ClientSecret"))
                    .Build();
                ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

                // Set up the Microsoft Graph service client with client credentials
                GraphServiceClient graphClient = new GraphServiceClient(authProvider);

                // Update the number of token
                int newTokenNumber = await UpdateCustomAttributeByUserId(graphClient, updateTokenBody.userId, log, tokens);

                return new OkObjectResult(new { userId = updateTokenBody.userId, tokenNumber = newTokenNumber });
            }
            else
            {
                return new NotFoundObjectResult(new { error = session.Id });
            }
        }

        private static async Task<int> UpdateCustomAttributeByUserId(GraphServiceClient graphClient, string userId, ILogger log, int addedTokenNumber)
        {
            string b2cExtensionAppClientId = Environment.GetEnvironmentVariable("b2cExtensionAppClientId");

            // Declare the names of the custom attributes
            const string customAttributeName = "ConversionToken";

            // Get the complete name of the custom attribute (Azure AD extension)
            B2cCustomAttributeHelper helper = new B2cCustomAttributeHelper(b2cExtensionAppClientId);
            string ConversionTokenAttributeName = helper.GetCompleteAttributeName(customAttributeName);

            // Get all users (one page)
            User existingUser = await graphClient.Users[userId]
                .Request()
                .Select($"id,displayName,identities,{ConversionTokenAttributeName}")
                .GetAsync();



            int existingToken = 0;

            if (existingUser.AdditionalData.ContainsKey(ConversionTokenAttributeName))
            {
                object exisitingTokenObject = existingUser.AdditionalData[ConversionTokenAttributeName];
                int.TryParse(exisitingTokenObject.ToString(), out existingToken);
            }

            // Fill custom attributes
            IDictionary<string, object> extensionInstance = new Dictionary<string, object>();
            extensionInstance.Add(ConversionTokenAttributeName, existingToken + addedTokenNumber);

            var user = new User
            {
                AdditionalData = extensionInstance
            };

            // Update user by object ID
            await graphClient.Users[userId]
               .Request()
               .UpdateAsync(user);

            log.LogInformation($"User with object ID '{userId}' successfully updated, with {existingToken + addedTokenNumber} tokens.");

            return existingToken + addedTokenNumber;
        }
    }

    public class UpdateTokenBody
    {
        public string userId { get; set; }
        public string sessionId { get; set; }
    }

    internal class B2cCustomAttributeHelper
    {
        internal readonly string _b2cExtensionAppClientId;

        internal B2cCustomAttributeHelper(string b2cExtensionAppClientId)
        {
            _b2cExtensionAppClientId = b2cExtensionAppClientId.Replace("-", "");
        }

        internal string GetCompleteAttributeName(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new System.ArgumentException("Parameter cannot be null", nameof(attributeName));
            }

            return $"extension_{_b2cExtensionAppClientId}_{attributeName}";
        }
    }
}


