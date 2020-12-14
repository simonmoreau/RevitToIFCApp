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
using Microsoft.WindowsAzure.Storage.Table;

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
        [Table("token", "token", "token", Connection = "StorageConnectionString")] Token token,
        [Table("token", Connection = "StorageConnectionString")] IAsyncCollector<Token> tokenTable,
        ILogger log)
    {
      try
      {
        if (token != null)
        {
          // Check if the token need to be refreshed
          if (DateTime.UtcNow - token.Timestamp.UtcDateTime > new TimeSpan(0, 50, 0))
          {
            // Refresh it
            Token refreshedToken = await RefreshForgeToken(tokenTable, log);
            return new OkObjectResult(refreshedToken.ForgeToken);
          }
          else
          {
            return new OkObjectResult(token.ForgeToken);
          }
        }
        else
        {
          // Get a token
          Token refreshedToken = await RefreshForgeToken(tokenTable, log);
          return new OkObjectResult(refreshedToken.ForgeToken);
        }
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }
    }

    private async Task<Token> RefreshForgeToken(IAsyncCollector<Token> tokenTable, ILogger log)
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

      return token;
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
