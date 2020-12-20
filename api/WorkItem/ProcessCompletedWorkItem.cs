using System;
using System.Threading.Tasks;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

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
        [Table("completedWorkItems", Connection = "StorageConnectionString")] IAsyncCollector<WorkItemStatusEntity> completedWorkItemsTable,
        [Table("completedWorkItems", Connection = "StorageConnectionString")] CloudTable completedWorkItemsCloudTable,
        [Table("createdWorkItems", Connection = "StorageConnectionString")] CloudTable createdWorkItemsCloudTable,
        ILogger log)
    {
      log.LogInformation($"C# Queue trigger function ProcessCompletedWorkItem processed");

      string userId = null;

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
          await createdWorkItemsCloudTable.ExecuteQuerySegmentedAsync(createdWorkItemsQuery, null))
      {
        userId = workItemStatusObject.UserId;
      }

      // Add to the completed work item

      if (userId != null)
      {
        WorkItemStatusEntity completedWorkItemStatusObject = Mappings.ToWorkItemStatusEntity(completedWorkItemStatus, userId);

        // Check if the completed work item has already been added to the table
        TableQuery<WorkItemStatusEntity> completedWorkItemsQuery = new TableQuery<WorkItemStatusEntity>().Where(
      TableQuery.CombineFilters(
          TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
              "workItems"),
          TableOperators.And,
          TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal,
              completedWorkItemStatus.Id)));

        TableQuerySegment<WorkItemStatusEntity> result = await completedWorkItemsCloudTable.ExecuteQuerySegmentedAsync(createdWorkItemsQuery, null);

        if (result.Results.Count == 0)
        {
          await completedWorkItemsTable.AddAsync(completedWorkItemStatusObject);

          // update the number of credits
          int newCreditsNumber = await _utilities.UpdateCustomAttributeByUserId(completedWorkItemStatusObject.UserId, -1);

          log.LogInformation($"The user ${completedWorkItemStatusObject.UserId} has now ${newCreditsNumber} credits.");
        }
      }

    }
  }
}
