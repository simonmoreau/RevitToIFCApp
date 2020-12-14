using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace api
{
  public static class GetConversionsNumber
  {
    [FunctionName("GetConversionsNumber")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "token")] HttpRequest req,
        ILogger log)
    {
      string userId = req.Query["id"];

      log.LogInformation("C# HTTP trigger function processed the UpdateConversionToken request.");

      // Initialize the client credential auth provider
      IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
          .Create(Environment.GetEnvironmentVariable("AppId"))
          .WithTenantId(Environment.GetEnvironmentVariable("TenantId"))
          .WithClientSecret(Environment.GetEnvironmentVariable("ClientSecret"))
          .Build();
      ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

      // Set up the Microsoft Graph service client with client credentials
      GraphServiceClient graphClient = new GraphServiceClient(authProvider);

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

      return new OkObjectResult(new { userId = userId, tokenNumber = existingToken });
    }
  }
}
