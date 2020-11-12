using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.Core;

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

        private readonly IWorkItemsApi _forgeApi;
        private readonly IEnginesApi _engineApi;
        public CreateWorkItem(IWorkItemsApi forgeApi, IEnginesApi engineApi)
        {
            this._forgeApi = forgeApi;
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
                

                ApiResponse<Autodesk.Forge.DesignAutomation.Model.Page<string>> page = await _engineApi.GetEnginesAsync();

                return new OkObjectResult(page);
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
