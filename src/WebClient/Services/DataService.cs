using System.Text;
using System.Text.Json;
using WebClient.Models;

namespace WebClient.Services
{
    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;

        public DataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateApplication(ForgeActivityForm forgeActivity)
        {
            StringContent forgeActivityContent = new StringContent(JsonSerializer.Serialize(forgeActivity), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"activities", forgeActivityContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wrong response");
            }

            return await JsonSerializer.DeserializeAsync<string>(await response.Content.ReadAsStreamAsync());
        }

        public async Task<ListForgeApplicationsVm> GetApplicationDetails()
        {
            ListForgeApplicationsVm? list = await JsonSerializer.DeserializeAsync<ListForgeApplicationsVm>
        (await _httpClient.GetStreamAsync($"forgeapplications"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return list;
        }

        public async Task<List<string>> GetUploadUrls()
        {
            List<string>? list = await JsonSerializer.DeserializeAsync<List<string>>
        (await _httpClient.GetStreamAsync($"files"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return list;
        }
    }
}
