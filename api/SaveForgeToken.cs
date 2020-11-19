using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using System.Threading.Tasks;
using Autodesk.Forge.Model;
using Microsoft.WindowsAzure.Storage.Table;

namespace api
{
    public class SaveForgeToken
    {
        // Initialize the oAuth 2.0 client configuration fron enviroment variables
        // you can also hardcode them in the code if you want in the placeholders below
        private static string FORGE_CLIENT_ID = Environment.GetEnvironmentVariable("FORGE_CLIENT_ID") ?? "your_client_id";
        private static string FORGE_CLIENT_SECRET = Environment.GetEnvironmentVariable("FORGE_CLIENT_SECRET") ?? "your_client_secret";
        private static Scope[] _scope = new Scope[] { Scope.DataRead, Scope.DataWrite };

        // Intialize the 2-legged oAuth 2.0 client.
        private readonly TwoLeggedApi _twoLeggedApi;

        public SaveForgeToken(TwoLeggedApi twoLeggedApi)
        {
            this._twoLeggedApi = twoLeggedApi;
        }

        [FunctionName("SaveForgeToken")]
        public async Task Run([TimerTrigger("0 * * * *")] TimerInfo myTimer,
        [Table("token")] IAsyncCollector<Token> tokenTable,
        ILogger log)
        {
            try
            {
                // Call the asynchronous version of the 2-legged client with HTTP information
                // HTTP information will help you to verify if the call was successful as well
                // as read the HTTP transaction headers.
                ApiResponse<dynamic> response = await _twoLeggedApi.AuthenticateAsyncWithHttpInfo(FORGE_CLIENT_ID, FORGE_CLIENT_SECRET, oAuthConstants.CLIENT_CREDENTIALS, _scope);
                ForgeToken forgeToken = (response.Data as DynamicJsonResponse).ToObject<ForgeToken>();

                Token token = new Token();
                token.ForgeToken = forgeToken;

                await tokenTable.AddAsync(token);

                log.LogInformation("C# timer trigger function SaveForgeToken saved a Forge Token");
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
            }

        }
    }

    public class ForgeToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }

    public class Token : TableEntity
    {        
        public Token()
        {
            this.RowKey = "token";
            this.PartitionKey = "token";
            this.ETag = "*";
        }
        public ForgeToken ForgeToken { get; set; }
    }
}
