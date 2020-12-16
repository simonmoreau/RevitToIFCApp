using System;
using System.IO;
using System.Collections.Generic;
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
  public class Utilities
  {
    private readonly GraphServiceClient _graphServiceClient;
    private readonly string _conversionCreditsAttributeName;
    public Utilities(GraphServiceClient graphServiceClient)
    {
      this._graphServiceClient = graphServiceClient;

      string b2cExtensionAppClientId = Environment.GetEnvironmentVariable("b2cExtensionAppClientId");

      // Declare the names of the custom attributes
      const string customAttributeName = "ConversionCredits";

      // Get the complete name of the custom attribute (Azure AD extension)
      B2cCustomAttributeHelper helper = new B2cCustomAttributeHelper(b2cExtensionAppClientId);
      _conversionCreditsAttributeName = helper.GetCompleteAttributeName(customAttributeName);

    }
    public async Task<int> GetConversionCredits(string userId)
    {
      // Get all users (one page)
      User existingUser = await this._graphServiceClient.Users[userId]
          .Request()
          .Select($"id,displayName,identities,{_conversionCreditsAttributeName}")
          .GetAsync();

      int existingCredits = 0;

      if (existingUser.AdditionalData.ContainsKey(_conversionCreditsAttributeName))
      {
        object exisitingCreditsObject = existingUser.AdditionalData[_conversionCreditsAttributeName];
        int.TryParse(exisitingCreditsObject.ToString(), out existingCredits);
      }

      return existingCredits;
    }

    public async Task<int> UpdateCustomAttributeByUserId(string userId, int addedCreditsNumber)
    {
      // Get all users (one page)
      User existingUser = await this._graphServiceClient.Users[userId]
          .Request()
          .Select($"id,displayName,identities,{_conversionCreditsAttributeName}")
          .GetAsync();

      int existingCredits = 0;

      if (existingUser.AdditionalData.ContainsKey(_conversionCreditsAttributeName))
      {
        object exisitingCreditsObject = existingUser.AdditionalData[_conversionCreditsAttributeName];
        int.TryParse(exisitingCreditsObject.ToString(), out existingCredits);
      }

      // Fill custom attributes
      IDictionary<string, object> extensionInstance = new Dictionary<string, object>();
      extensionInstance.Add(_conversionCreditsAttributeName, existingCredits + addedCreditsNumber);

      var user = new User
      {
        AdditionalData = extensionInstance
      };

      // Update user by object ID
      await this._graphServiceClient.Users[userId]
         .Request()
         .UpdateAsync(user);


      return existingCredits + addedCreditsNumber;
    }
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