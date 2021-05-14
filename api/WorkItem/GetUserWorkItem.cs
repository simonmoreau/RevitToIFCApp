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
using Microsoft.Azure.Cosmos.Table;

namespace api
{
  public static class GetUserWorkItem
  {
    [FunctionName("GetUserWorkItem")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{userId}/workitems")] HttpRequest req,
        [Table("workItems", Connection = "StorageConnectionString")] CloudTable workItemsCloudTable,
        string userId,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed the GetUserWorkItem request.");

      try
      {
        // Get all work item for a given user
        TableQuery<WorkItemStatusEntity> createdWorkItemsQuery = new TableQuery<WorkItemStatusEntity>().Where(
            TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "workItems"),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId)
                ));

        List<WorkItemStatusEntity> WorkItemStatusEntities = new List<WorkItemStatusEntity>();

        // Execute the query and loop through the results
        foreach (WorkItemStatusEntity workItemStatusObject in
            await workItemsCloudTable.ExecuteQuerySegmentedAsync(createdWorkItemsQuery, null))
        {
          TimeSpan? timeSpan = DateTime.Now - workItemStatusObject.TimeDownloadStarted;
          TimeSpan thirtyDays = new TimeSpan(30, 0, 0, 0);

          if (timeSpan != null)
          {
            if (timeSpan < thirtyDays)
            {
              WorkItemStatusEntities.Add(workItemStatusObject);
            }
          }
        }

        return new OkObjectResult(WorkItemStatusEntities);
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }

    }
  }
}
