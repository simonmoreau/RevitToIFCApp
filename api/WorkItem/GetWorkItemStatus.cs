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
            [Table("workItems", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatusObject> workItemsTable,
            [Queue("completedWorkItems", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatusObject> completedWorkItemsQueue,
            string workItemId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed the GetWorkItemStatus request.");

            try
            {
                Autodesk.Forge.Core.ApiResponse<WorkItemStatus> workItemResponse = await _workItemApi.GetWorkitemStatusAsync(workItemId);

                if (workItemResponse.Content.Status == Status.Success)
                {
                    WorkItemStatusObject resultingWorkItemStatus = new WorkItemStatusObject(workItemResponse.Content);
                    await workItemsTable.AddAsync(resultingWorkItemStatus);
                    await completedWorkItemsQueue.AddAsync(resultingWorkItemStatus);
                }
                
                return new OkObjectResult(workItemResponse.Content);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
