using System.Text.Json.Serialization;

namespace WebClient.Models
{
    public class CompleteUploadResponse
    {
        [JsonPropertyName("bucketKey")]
        public string BucketKey { get; set; }

        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }

        [JsonPropertyName("objectKey")]
        public string ObjectKey { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
    }
}
