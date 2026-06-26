using Azure;
using Azure.Data.Tables;

namespace Domain.Entities
{
    public class ConversionCheckoutSession : ITableEntity
    {
        public ConversionCheckoutStatus Status { get; set; } = ConversionCheckoutStatus.Started;
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public enum ConversionCheckoutStatus
    {
        Complete,
        Started,
        Failed
    }
}
