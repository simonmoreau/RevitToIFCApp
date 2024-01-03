using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkItem : ITableEntity
    {
        public string? Progress { get; set; }
        public string? ReportUrl { get; set; }
        public string? FileUrl { get; set; }
        public string? InputUrl { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public DateTime TimeQueued { get; set; }
        public int Size { get; set; }
        public string? Version { get; set; }
        public string? FileName { get; set; }
        public DateTime? TimeDownloadStarted { get; set; }
        public DateTime? TimeInstructionsStarted { get; set; }
        public DateTime? TimeInstructionsEnded { get; set; }
        public DateTime? TimeUploadEnded { get; set; }
        public DateTime? TimeFinished { get; set; }
        public long? BytesDownloaded { get; set; }
        public long? BytesUploaded { get; set; }
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
