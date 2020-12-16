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
  public static class Utilities
  {
    public static async Task<int> GetConversionCredits(string userId)
    {
      string b2cExtensionAppClientId = Environment.GetEnvironmentVariable("b2cExtensionAppClientId");

      // Declare the names of the custom attributes
      const string customAttributeName = "ConversionCredits";

      // Get the complete name of the custom attribute (Azure AD extension)
      B2cCustomAttributeHelper helper = new B2cCustomAttributeHelper(b2cExtensionAppClientId);
      string ConversionCreditsAttributeName = helper.GetCompleteAttributeName(customAttributeName);

      // Get all users (one page)
      User existingUser = await this._graphServiceClient.Users[userId]
          .Request()
          .Select($"id,displayName,identities,{ConversionCreditsAttributeName}")
          .GetAsync();

      int existingCredits = 0;

      if (existingUser.AdditionalData.ContainsKey(ConversionCreditsAttributeName))
      {
        object exisitingCreditsObject = existingUser.AdditionalData[ConversionCreditsAttributeName];
        int.TryParse(exisitingCreditsObject.ToString(), out existingCredits);
      }

      return existingCredits;
    }
  }
}