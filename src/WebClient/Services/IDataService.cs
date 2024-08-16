using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();

        Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber, string objectKey);
        Task<Signeds3downloadResponse> GetDownloadUrl(string objectKey, string fileName);

        Task<CompleteUploadResponse> CompleteUpload(string uploadKey, long? size, List<string> eTags, string objectKey);

        Task<WorkItemStatus> CreateWorkItem(string objectKey, string revitVersion, string fileName);

        Task<WorkItemStatus> GetWorkItemStatus(string workItemId);

        Task<string> CreateApplication(ForgeActivityForm forgeActivity);
    }
}
