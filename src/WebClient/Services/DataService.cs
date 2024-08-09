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

        public async Task<CompleteUploadResponse> CompleteUpload(string uploadKey, long? size, List<string> eTags)
        {
            object body = new { uploadKey = uploadKey, size = size, eTags = eTags };

            StringContent bodyContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"files/complete", bodyContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wrong response");
            }

            return await JsonSerializer.DeserializeAsync<CompleteUploadResponse>(await response.Content.ReadAsStreamAsync());
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

        public async Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber)
        {
            Signeds3uploadResponse? list = await JsonSerializer.DeserializeAsync<Signeds3uploadResponse>
        (await _httpClient.GetStreamAsync($"files?chunksNumber={chunksNumber}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return list;
        }
    }
}
