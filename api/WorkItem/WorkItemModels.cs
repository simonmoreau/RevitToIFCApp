using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.WindowsAzure.Storage.Table;
using System;


namespace api
{
  public class WorkItemDescription
  {
    public string inputObjectName { get; set; }
    public string outputObjectName { get; set; }
    public string activityId { get; set; }
    public string userId { get; set; }
  }

  public class WorkItemStatusEntity : TableEntity
  {
    public string Progress { get; set; }
    public string ReportUrl { get; set; }
    public Statistics Stats { get; set; }
    public Autodesk.Forge.DesignAutomation.Model.Status Status { get; set; }
    public string UserId { get; set; }

  }

  public class WorkItemStatusResponse
  {
    public string WorkItemId { get; set; }
    public string OutputUrl { get; set; }
    public WorkItemCreationStatus WorkItemCreationStatus { get; set; }
  }

  public enum WorkItemCreationStatus
  {
    Created, NotEnoughCredit, Error
  }


  public static class Mappings
  {
    public static WorkItemStatusEntity ToWorkItemStatusEntity(this WorkItemStatus workItemStatus, string userId)
    {
      return new WorkItemStatusEntity()
      {
        UserId = userId,
        PartitionKey = "workItems",
        RowKey = workItemStatus.Id,
        ETag = "*",
        Progress = workItemStatus.Progress,
        ReportUrl = workItemStatus.ReportUrl,
        Stats = workItemStatus.Stats,
        Status = workItemStatus.Status,
      };
    }

    // public static WorkItemStatus ToWorkItemStatus(this WorkItemStatusEntity todo)
    // {
    //     return new WorkItemStatus()
    //     {
    //         Id = todo.RowKey,
    //         CreatedTime = todo.CreatedTime,
    //         IsCompleted = todo.IsCompleted,
    //         TaskDescription = todo.TaskDescription
    //     };
    // }

  }
}