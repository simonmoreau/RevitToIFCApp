using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Autodesk.Forge.Model;

namespace api
{
    public class GetForgeUploadToken
    {
        // Initialize the oAuth 2.0 client configuration fron enviroment variables
        // you can also hardcode them in the code if you want in the placeholders below
        private static string FORGE_CLIENT_ID = Environment.GetEnvironmentVariable("FORGE_CLIENT_ID") ?? "your_client_id";
        private static string FORGE_CLIENT_SECRET = Environment.GetEnvironmentVariable("FORGE_CLIENT_SECRET") ?? "your_client_secret";
        private static Scope[] _scope = new Scope[] { Scope.DataRead, Scope.DataWrite };

        // Intialize the 2-legged oAuth 2.0 client.
        private readonly TwoLeggedApi _twoLeggedApi;

        public GetForgeUploadToken(TwoLeggedApi twoLeggedApi)
        {
            this._twoLeggedApi = twoLeggedApi;
        }

        [FunctionName("GetForgeUploadToken")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "uploadToken")] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Call the asynchronous version of the 2-legged client with HTTP information
                // HTTP information will help you to verify if the call was successful as well
                // as read the HTTP transaction headers.
                ApiResponse<dynamic> bearer = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, oAuthConstants.CLIENT_CREDENTIALS, _scope);
                //if ( bearer.StatusCode != 200 )
                //	throw new Exception ("Request failed! (with HTTP response " + bearer.StatusCode + ")") ;

                // The JSON response from the oAuth server is the Data variable and has been
                // already parsed into a DynamicDictionary object.

                return new OkObjectResult(bearer.Data);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
            
        }
    }
}
