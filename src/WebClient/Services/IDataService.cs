using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();

        Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber);

        Task<CompleteUploadResponse> CompleteUpload(string uploadKey, long? size, List<string> eTags);

        Task<string> CreateApplication(ForgeActivityForm forgeActivity);
    }
}
