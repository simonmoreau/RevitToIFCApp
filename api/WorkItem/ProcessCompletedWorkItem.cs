using System;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using System.IO;

namespace api
{
  public class ProcessCompletedWorkItem
  {
    private readonly Utilities _utilities;
    public ProcessCompletedWorkItem(Utilities utilities)
    {
      this._utilities = utilities;
    }

    [FunctionName("ProcessCompletedWorkItem")]
    public async Task Run(
        [QueueTrigger("completedworkitems", Connection = "StorageConnectionString")] WorkItemStatus completedWorkItemStatus,
        // [Table("completedWorkItems", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatusEntity> completedWorkItemsTable,
        // [Table("completedWorkItems", Connection = "StorageConnectionString")] CloudTable completedWorkItemsCloudTable,
        [Blob("reports", FileAccess.Write, Connection = "StorageConnectionString")] CloudBlobContainer reportsCloudBlobContainer,
        [Table("workItems", Connection = "StorageConnectionString")] CloudTable workItemsCloudTable,
        [Queue("failedConversions", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatusEntity> failedConversionsQueue,
        ILogger log)
    {
      log.LogInformation($"C# Queue trigger function ProcessCompletedWorkItem processed");

      string userId = null;
      string fileVersion = null;
      string fileName = null;
      int fileSize = 0;
      string fileUrl = null;
      string inputUrl = null;

      // Get user id
      TableQuery<WorkItemStatusEntity> createdWorkItemsQuery = new TableQuery<WorkItemStatusEntity>().Where(
    TableQuery.CombineFilters(
        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
            "workItems"),
        TableOperators.And,
        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,
            completedWorkItemStatus.Id)));

      // Execute the query and loop through the results
      foreach (WorkItemStatusEntity workItemStatusObject in
          await workItemsCloudTable.ExecuteQuerySegmentedAsync(createdWorkItemsQuery, null))
      {
        userId = workItemStatusObject.UserId;
        fileSize = workItemStatusObject.Size;
        fileVersion = workItemStatusObject.Version;
        fileName = workItemStatusObject.FileName;
        fileUrl = workItemStatusObject.FileUrl;
        inputUrl = workItemStatusObject.InputUrl;
      }

      // Add to the completed work item

      if (userId != null)
      {
        WorkItemStatusEntity completedWorkItemStatusObject = Mappings.ToWorkItemStatusEntity(
          completedWorkItemStatus,
          userId,
          fileSize,
          fileVersion,
          fileName,
          fileUrl,
          inputUrl);

        // Save the report in Azure
        await reportsCloudBlobContainer.CreateIfNotExistsAsync();

        // Create a SAS token that's valid for one hour.
        SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
        sasConstraints.SharedAccessExpiryTime = DateTime.UtcNow.AddDays(30);
        sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

        Uri reportUri = new Uri(completedWorkItemStatusObject.ReportUrl);
        string reportName = Path.GetFileNameWithoutExtension(completedWorkItemStatusObject.FileName) + ".txt";
        CloudBlockBlob target = reportsCloudBlobContainer.GetBlockBlobReference(reportName);
        await target.StartCopyAsync(reportUri);

        // Get its url
        string reportUrl = target.Uri.AbsoluteUri + target.GetSharedAccessSignature(sasConstraints);
        completedWorkItemStatusObject.ReportUrl = reportUrl;

        TableOperation tableOperation = TableOperation.InsertOrMerge(completedWorkItemStatusObject);
        await workItemsCloudTable.ExecuteAsync(tableOperation);

        if (completedWorkItemStatusObject.Status == "Success")
        {
          // update the number of credits
          int newCreditsNumber = await _utilities.UpdateCustomAttributeByUserId(completedWorkItemStatusObject.UserId, -1);

          log.LogInformation($"The user {completedWorkItemStatusObject.UserId} has now {newCreditsNumber} credits.");
        }
        else
        {
          await failedConversionsQueue.AddAsync(completedWorkItemStatusObject);
        }
      }

    }
  }
}
