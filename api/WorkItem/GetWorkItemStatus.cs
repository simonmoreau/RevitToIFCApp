using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DA = Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;

namespace api
{
  public class GetWorkItemStatus
  {
    private readonly DA.IWorkItemsApi _workItemApi;
    private readonly DA.IEnginesApi _engineApi;
    public GetWorkItemStatus(DA.IWorkItemsApi workItemApi, DA.IEnginesApi engineApi)
    {
      this._workItemApi = workItemApi;
      this._engineApi = engineApi;
    }

    [FunctionName("GetWorkItemStatus")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "workitem/{workItemId}")] HttpRequest req,
        [Queue("completedworkitems", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatus> completedWorkItemsQueue,
        string workItemId,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed the GetWorkItemStatus request.");

      try
      {
        Autodesk.Forge.Core.ApiResponse<WorkItemStatus> workItemResponse = await _workItemApi.GetWorkitemStatusAsync(workItemId);

        WorkItemStatus workItemStatus = workItemResponse.Content;

        if (workItemStatus.Status == Status.Pending || workItemStatus.Status == Status.Inprogress)
        {
          // check if the workItem run for less than a hour
          TimeSpan? duration = DateTime.Now - workItemStatus.Stats.TimeDownloadStarted;

          if (duration != null)
          {
            if (duration > new TimeSpan(0, 55, 0))
            {
              await _workItemApi.DeleteWorkItemAsync(workItemId);
            }
          }

        }
        else
        {
          await completedWorkItemsQueue.AddAsync(workItemStatus);
        }

        return new OkObjectResult(workItemStatus);
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }
    }
  }
}
