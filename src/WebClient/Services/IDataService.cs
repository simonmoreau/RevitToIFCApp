using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();

        Task<List<string>> GetUploadUrls();

        Task<string> CreateApplication(ForgeActivityForm forgeActivity);
    }
}
