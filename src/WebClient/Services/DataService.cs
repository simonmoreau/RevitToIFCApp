using System.Text.Json;
using WebClient.Models;

namespace WebClient.Services
{
    public class DataService : IDataService
    {
        private readonly HttpClient? _httpClient;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ListForgeApplicationsVm> GetApplicationDetails()
        {
            ListForgeApplicationsVm? list = await JsonSerializer.DeserializeAsync<ListForgeApplicationsVm>
        (await _httpClient.GetStreamAsync($"forgeapplications"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return list;
        }
    }
}
