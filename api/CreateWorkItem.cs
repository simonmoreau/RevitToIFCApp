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
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation.Model;

namespace api
{
    public class CreateWorkItem
    {
        // // Initialize the oAuth 2.0 client configuration fron enviroment variables
        // // you can also hardcode them in the code if you want in the placeholders below
        // private static string FORGE_CLIENT_ID = Environment.GetEnvironmentVariable("FORGE_CLIENT_ID") ?? "your_client_id";
        // private static string FORGE_CLIENT_SECRET = Environment.GetEnvironmentVariable("FORGE_CLIENT_SECRET") ?? "your_client_secret";
        // private static Scope[] _scope = new Scope[] { Scope.DataRead, Scope.DataWrite };

        // // Intialize the 2-legged oAuth 2.0 client.
        // private static TwoLeggedApi _twoLeggedApi = new TwoLeggedApi();

        private readonly IWorkItemsApi _workItemApi;
        private readonly IEnginesApi _engineApi;
        public CreateWorkItem(IWorkItemsApi workItemApi, IEnginesApi engineApi)
        {
            this._workItemApi = workItemApi;
            this._engineApi = engineApi;
        }

        [FunctionName("CreateWorkItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workitems")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed CreateWorkItem.");

            try
            {
                string downloadUrl = "https://developer.api.autodesk.com/oss/v2/signedresources/fb05b392-76d9-4324-9e06-ef2fb726d6c5?region=US";
                string uploadURl = "https://developer.api.autodesk.com/oss/v2/signedresources/ee67a183-c8f2-42f4-a85e-58806b7414e5?region=US";
                var workItem = new WorkItem()
                {
                    ActivityId = "RevitToIFC.RevitToIFCActivity+test",
                    Arguments = new Dictionary<string, IArgument>
                    {
                        { "rvtFile",  new XrefTreeArgument() { Url = downloadUrl } },
                        // { "params", new StringArgument() { Value = "{'ScheduleName':'WallSchedule.csv'}" }},
                        { "result", new XrefTreeArgument { Verb=Verb.Put, Url = uploadURl } }
                    }
                };
                
                ApiResponse<WorkItemStatus> workItemStatusResponse = await _workItemApi.CreateWorkItemAsync(workItem);
                // ApiResponse<Page<string>> page = await _engineApi.GetEnginesAsync();

                return new OkObjectResult(workItemStatusResponse.Content);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }

    public class RvtFile
    {
        public string url { get; set; }
    }

    public class Param
    {
        public string url { get; set; }
    }

    public class Result
    {
        public string verb { get; set; }
        public string url { get; set; }
    }

    public class Arguments
    {
        public RvtFile rvtFile { get; set; }
        public Param param { get; set; }
        public Result result { get; set; }
    }

    public class WorkItemDescription
    {
        public string activityId { get; set; }
        public Arguments arguments { get; set; }
    }
}
