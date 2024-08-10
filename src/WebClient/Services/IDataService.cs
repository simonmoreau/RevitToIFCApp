using Autodesk.Forge.DesignAutomation.Model;
using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();

        Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber, string objectKey);

        Task<CompleteUploadResponse> CompleteUpload(string uploadKey, long? size, List<string> eTags, string objectKey);

        Task<WorkItemStatus> CreateWorkItem(string objectKey, string activityId);

        Task<string> CreateApplication(ForgeActivityForm forgeActivity);
    }
}
