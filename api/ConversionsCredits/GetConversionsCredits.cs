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
  public class GetConversionsCredits
  {
    private readonly GraphServiceClient _graphServiceClient;
    public GetConversionsCredits(GraphServiceClient graphServiceClient)
    {
      this._graphServiceClient = graphServiceClient;
    }
    [FunctionName("GetConversionsCredits")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "credits")] HttpRequest req,
        ILogger log)
    {

      try
      {
        string userId = req.Query["id"];

        log.LogInformation("C# HTTP trigger function processed the GetConversionsCredits request.");

        int existingCredits = await Utilities.GetConversionCredits(userId);

        return new OkObjectResult(new { userId = userId, creditsNumber = existingCredits });
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }
    }
  }
}
