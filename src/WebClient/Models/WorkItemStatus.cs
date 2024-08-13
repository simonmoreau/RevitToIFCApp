using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebClient.Models
{
    public class WorkItemStatus
    {
        [JsonPropertyName("status")]
        public Status Status { get; set; }

        [JsonPropertyName("progress")]
        public string Progress { get; set; }

        [JsonPropertyName("reportUrl")]
        public string ReportUrl { get; set; }

        [JsonPropertyName("debugInfoUrl")]
        public Uri DebugInfoUrl { get; private set; }

        [JsonPropertyName("stats")]
        public Statistics Stats { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public enum Status
    {
        [EnumMember(Value = "pending")]
        Pending = 1,
        [EnumMember(Value = "inprogress")]
        Inprogress = 2,
        [EnumMember(Value = "cancelled")]
        Cancelled = 3,
        [EnumMember(Value = "failedLimitDataSize")]
        [Obsolete]
        FailedLimitDataSize = 4,
        [EnumMember(Value = "failedLimitProcessingTime")]
        FailedLimitProcessingTime = 5,
        [EnumMember(Value = "failedDownload")]
        FailedDownload = 6,
        [EnumMember(Value = "failedInstructions")]
        FailedInstructions = 7,
        [EnumMember(Value = "failedUpload")]
        FailedUpload = 8,
        [EnumMember(Value = "failedUploadOptional")]
        FailedUploadOptional = 10,
        [EnumMember(Value = "success")]
        Success = 9
    }

    public class Statistics
    {
        [JsonPropertyName("timeQueued")]
        public DateTime TimeQueued { get; set; }

        [JsonPropertyName("timeDownloadStarted")]
        public DateTime? TimeDownloadStarted { get; set; }

        [JsonPropertyName("timeInstructionsStarted")]
        public DateTime? TimeInstructionsStarted { get; set; }

        [JsonPropertyName("timeInstructionsEnded")]
        public DateTime? TimeInstructionsEnded { get; set; }

        [JsonPropertyName("timeUploadEnded")]
        public DateTime? TimeUploadEnded { get; set; }

        [JsonPropertyName("timeFinished")]
        public DateTime? TimeFinished { get; set; }

        [JsonPropertyName("bytesDownloaded")]
        public long? BytesDownloaded { get; set; }

        [JsonPropertyName("bytesUploaded")]
        public long? BytesUploaded { get; set; }
    }
}
