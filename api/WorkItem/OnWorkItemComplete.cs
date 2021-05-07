using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Autodesk.Forge.DesignAutomation.Model;

namespace api
{
  public static class OnWorkItemComplete
  {
    [FunctionName("OnWorkItemComplete")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "onworkitemcomplete")] HttpRequest req,
        [Queue("completedworkitems", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatus> completedWorkItemsQueue,
        ILogger log)
    {

      log.LogInformation("C# HTTP trigger function processed OnWorkItemComplete.");

      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        WorkItemStatus workItemStatus = JsonConvert.DeserializeObject<WorkItemStatus>(requestBody);

        if (workItemStatus != null)
        {
            await completedWorkItemsQueue.AddAsync(workItemStatus);
          return new OkObjectResult(workItemStatus);
        }
        else
        {
          return new BadRequestObjectResult(null);
        }
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }
    }
  }
}
