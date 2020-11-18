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
using DA = Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;

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

        private readonly DA.IWorkItemsApi _workItemApi;
        private readonly DA.IEnginesApi _engineApi;
        public CreateWorkItem(DA.IWorkItemsApi workItemApi, DA.IEnginesApi engineApi)
        {
            this._workItemApi = workItemApi;
            this._engineApi = engineApi;
        }

        [FunctionName("CreateWorkItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "workitem")] HttpRequest req,
            [Table("token", "token", "token")] Token token,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed CreateWorkItem.");

            try
            {
                Configuration.Default.AccessToken = token.ForgeToken.access_token;
                // Parse the body of the request
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                WorkItemDescription workItemDescription = JsonConvert.DeserializeObject<WorkItemDescription>(requestBody);

                // Create two signed URLs to upload the file to the activity and download the result
                ObjectsApi apiInstance = new ObjectsApi();
                string bucketKey = Environment.GetEnvironmentVariable("ossBucketKey");  // string | URL-encoded bucket key
                string inputObjectName = workItemDescription.inputObjectName;  // string | URL-encoded object name
                string outputObjectName = workItemDescription.outputObjectName;
                PostBucketsSigned postBucketsSigned = new PostBucketsSigned(60);

                DynamicJsonResponse dynamicJsonResponseDownload = await (apiInstance.CreateSignedResourceAsync(bucketKey, inputObjectName, postBucketsSigned, "read"));
                PostObjectSigned downloadSigned = dynamicJsonResponseDownload.ToObject<PostObjectSigned>();
                DynamicJsonResponse dynamicJsonResponseUpload = await apiInstance.CreateSignedResourceAsync(bucketKey, outputObjectName, postBucketsSigned, "readwrite");
                PostObjectSigned uploadSigned = dynamicJsonResponseUpload.ToObject<PostObjectSigned>();

                var workItem = new Autodesk.Forge.DesignAutomation.Model.WorkItem()
                {
                    ActivityId = "RevitToIFC.RevitToIFCActivity+test",
                    Arguments = new Dictionary<string, IArgument>
                    {
                        { "rvtFile",  new XrefTreeArgument() { Url = downloadSigned.SignedUrl } },
                        { "result", new XrefTreeArgument { Verb=Verb.Put, Url = uploadSigned.SignedUrl } }
                    }
                };

                Autodesk.Forge.Core.ApiResponse<WorkItemStatus> workItemResponse = await _workItemApi.CreateWorkItemAsync(workItem);
                // ApiResponse<Page<string>> page = await _engineApi.GetEnginesAsync();

                WorkItemStatusResponse workItemStatusResponse = new WorkItemStatusResponse()
                {
                    WorkItemId = workItemResponse.Content.Id,
                    OutputUrl = uploadSigned.SignedUrl
                };

                return new OkObjectResult(workItemStatusResponse);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }

    public class WorkItemDescription
    {
        public string inputObjectName { get; set; }
        public string outputObjectName {get;set;}
    }

    public class WorkItemStatusResponse
    {
        public string WorkItemId { get; set; }
        public string OutputUrl { get; set; }
    }
}
