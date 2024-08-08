using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();

        Task<List<string>> GetUploadUrls(int chunksNumber);

        Task<string> CreateApplication(ForgeActivityForm forgeActivity);
    }
}
