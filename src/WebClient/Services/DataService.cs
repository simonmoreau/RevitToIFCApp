using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
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

        public async Task<CompleteUploadResponse> CompleteUpload(string uploadKey, long? size, List<string> eTags, string objectKey)
        {
            object body = new { uploadKey = uploadKey, size = size, eTags = eTags , objectKey  = objectKey };

            StringContent bodyContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"files/complete", bodyContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wrong response");
            }

            return await JsonSerializer.DeserializeAsync<CompleteUploadResponse>(await response.Content.ReadAsStreamAsync());
        }

        public async Task<WorkItemStatus> CreateWorkItem(string objectKey, string revitVersion, string fileName)
        {
            ConversionProperties conversionProperties = new ConversionProperties() { FileName = fileName };
            object body = new { objectKey = objectKey, revitVersion = revitVersion, conversionProperties = conversionProperties };

            StringContent bodyContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"files/workItem", bodyContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Wrong response");
            }

            return await JsonSerializer.DeserializeAsync<WorkItemStatus>(await response.Content.ReadAsStreamAsync());
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
            try
            {
                ListForgeApplicationsVm? list = await JsonSerializer.DeserializeAsync<ListForgeApplicationsVm>
(await _httpClient.GetStreamAsync($"forgeapplications"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return list;
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
                return null;
            }
        }

        public async Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber, string objectKey)
        {
            Signeds3uploadResponse? list = await JsonSerializer.DeserializeAsync<Signeds3uploadResponse>
        (await _httpClient.GetStreamAsync($"files?chunksNumber={chunksNumber}&objectKey={objectKey}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return list;
        }

        public async Task<WorkItemStatus> GetWorkItemStatus(string workItemId)
        {
            WorkItemStatus status = await JsonSerializer.DeserializeAsync<WorkItemStatus>
        (await _httpClient.GetStreamAsync($"files/status?id={workItemId}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return status;
        }

        public async Task<Signeds3downloadResponse> GetDownloadUrl(string objectKey, string fileName)
        {
            Signeds3downloadResponse? list = await JsonSerializer.DeserializeAsync<Signeds3downloadResponse>
        (await _httpClient.GetStreamAsync($"files/download?objectKey={objectKey}&fileName={fileName}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return list;
        }

        public async Task<string> GetMe()
        {
            string response = await _httpClient.GetStringAsync($"users");

            return response;
        }
    }
}
