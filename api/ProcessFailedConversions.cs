using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;

namespace api
{
  public class ProcessFailedConversions
  {
    [FunctionName("ProcessFailedConversions")]
    public static async Task Run([QueueTrigger("failedConversions", Connection = "StorageConnectionString")] WorkItemStatusEntity failedConversion,
    [Blob("failed-conversions", FileAccess.Write, Connection = "StorageConnectionString")] CloudBlobContainer faildedConversionCloudBlobContainer,
    ILogger log)
    {
      log.LogInformation($"C# Queue trigger function processed: failedConversion");

      // Save the files in Azure
      await faildedConversionCloudBlobContainer.CreateIfNotExistsAsync();

      Uri fileUri = new Uri(failedConversion.InputUrl);
      CloudBlockBlob target = faildedConversionCloudBlobContainer.GetBlockBlobReference(failedConversion.FileName);
      // try
      // {
      //     string response = await target.StartCopyAsync(fileUri);
      // }
      // catch (System.Exception ex)
      // {
      //     System.Diagnostics.Debug.WriteLine(ex.Message);
      //     throw;
      // }
      
    }
  }
}
