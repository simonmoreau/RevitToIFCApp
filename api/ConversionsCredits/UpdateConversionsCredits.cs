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
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace api
{
  public class UpdateConversionsCredits
  {
    private readonly Utilities _utilities;
    public UpdateConversionsCredits(Utilities utilities)
    {
      this._utilities = utilities;
    }

    [FunctionName("UpdateConversionsCredits")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "credits")] HttpRequest req,
        ILogger log)
    {
      try
      {
        log.LogInformation("C# HTTP trigger function processed the UpdateConversionsCredits request.");

        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("stripe_api_key");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        UpdateCreditsBody updateCreditsBody = JsonConvert.DeserializeObject<UpdateCreditsBody>(requestBody);

        SessionService sessionService = new SessionService();
        Session session = sessionService.Get(updateCreditsBody.sessionId);

        if (session != null)
        {
          // Get the product/price payed
          int credits = Convert.ToInt32(Math.Floor(Convert.ToDecimal(session.AmountTotal) / 100));

          // Update the number of credits
          int newCreditsNumber = await _utilities.UpdateCustomAttributeByUserId(updateCreditsBody.userId, credits);

          log.LogInformation($"User with object ID '{updateCreditsBody.userId}' successfully updated, with {newCreditsNumber} credits.");

          return new OkObjectResult(new { userId = updateCreditsBody.userId, creditsNumber = newCreditsNumber });
        }
        else
        {
          return new NotFoundObjectResult(new { error = session.Id });
        }
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }

    }
  }

  public class UpdateCreditsBody
  {
    public string userId { get; set; }
    public string sessionId { get; set; }
  }
}


