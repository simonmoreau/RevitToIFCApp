using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.Azure.Cosmos.Table;
using System;


namespace api
{
  public class WorkItemDescription
  {
    public string inputObjectName { get; set; }
    public string outputObjectName { get; set; }
    public string activityId { get; set; }
    public string userId { get; set; }
    public int fileSize {get;set;}
    public string version { get; set; }
    public string fileName { get; set; }
  }

  public class WorkItemStatusEntity : TableEntity
  {
    public string Progress { get; set; }
    public string ReportUrl { get; set; }
    public string FileUrl { get; set; }
    public string InputUrl {get;set;}
    public Statistic Stats { get; set; }
    public string Status { get; set; }
    public string UserId { get; set; }
    public DateTime TimeQueued { get; set; }
    public int Size {get;set;}
    public string Version { get; set; }
    public string FileName { get; set; }
    public DateTime? TimeDownloadStarted { get; set; }
    public DateTime? TimeInstructionsStarted { get; set; }
    public DateTime? TimeInstructionsEnded { get; set; }
    public DateTime? TimeUploadEnded { get; set; }
    public DateTime? TimeFinished { get; set; }
    public long? BytesDownloaded { get; set; }
    public long? BytesUploaded { get; set; }

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

  public class Statistic
  {

  }


  public static class Mappings
  {
    public static WorkItemStatusEntity ToWorkItemStatusEntity(
      this WorkItemStatus workItemStatus, 
      string userId, 
      int size, 
      string version, 
      string fileName, 
      string fileUrl,
      string inputUrl)
    {
      return new WorkItemStatusEntity()
      {
        UserId = userId,
        PartitionKey = "workItems",
        RowKey = workItemStatus.Id,
        ETag = "*",
        Progress = workItemStatus.Progress,
        ReportUrl = workItemStatus.ReportUrl,
        FileUrl = fileUrl,
        InputUrl = inputUrl,
        Stats = ToStatistic(workItemStatus.Stats),
        Status = workItemStatus.Status.ToString(),
        TimeQueued = workItemStatus.Stats.TimeQueued,
        Size = size,
        Version = version,
        FileName = fileName,
        TimeDownloadStarted = workItemStatus.Stats.TimeDownloadStarted,
        TimeInstructionsStarted = workItemStatus.Stats.TimeInstructionsStarted,
        TimeInstructionsEnded = workItemStatus.Stats.TimeInstructionsEnded,
        TimeUploadEnded = workItemStatus.Stats.TimeUploadEnded,
        TimeFinished = workItemStatus.Stats.TimeFinished,
        BytesDownloaded = workItemStatus.Stats.BytesDownloaded,
        BytesUploaded = workItemStatus.Stats.BytesUploaded,
      };
    }

    public static Statistic ToStatistic(Statistics statistics)
    {
      return new Statistic()
      {

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