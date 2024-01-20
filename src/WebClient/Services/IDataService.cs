using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();

        Task<string> CreateApplication(ForgeActivityForm forgeActivity);
    }
}
