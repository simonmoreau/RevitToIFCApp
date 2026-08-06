using Autodesk.Authentication.Model;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss.Model;

namespace Application.Common.Services
{
    public interface IAutodeskPlatformService
    {
        Task<string> GetAccessTokenAsync(List<Scopes> scopes);

        Task<Signeds3uploadResponse> SignedS3UploadAsync(string bucketKey, string objectKey, int parts, string? uploadKey);

        Task<BucketObjects> GetObjectsAsync(string bucketKey);

        Task<Signeds3downloadResponse> SignedS3DownloadAsync(string bucketKey, string objectKey, string responseContentDisposition);

        Task<HttpResponseMessage> CompleteSignedS3UploadAsync(string bucketKey, string objectKey, Completes3uploadBody completes3UploadBody);

        Task<Bucket> CreateBucketAsync(CreateBucketsPayload payload, Region region);

        Task<WorkItemStatus> CreateWorkItemAsync(WorkItem workItem);
    }
}
