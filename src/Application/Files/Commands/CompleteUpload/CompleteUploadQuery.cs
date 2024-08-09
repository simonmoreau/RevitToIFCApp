using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Files.Commands.CompleteUpload
{
    public class CompleteUploadQuery : IRequest<CompleteUploadResponse>
    {
        public readonly string UploadKey;
        public readonly long? Size;
        public readonly List<string> ETags;

        public CompleteUploadQuery(string uploadKey, long? size, List<string> eTags)
        {
            UploadKey = uploadKey;
            Size = size;
            ETags = eTags;
        }
    }

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
