using WebClient.Models;

namespace WebClient.Services
{
    public interface IDataService
    {
        Task<ListForgeApplicationsVm> GetApplicationDetails();
    }
}
