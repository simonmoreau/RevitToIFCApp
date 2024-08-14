using System.Net.Http.Headers;

namespace WebClient.Services
{
    public class UploadService : IUploadService
    {
        private readonly HttpClient _httpClient;

        public UploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadChunk(MemoryStream chunck, string url)
        {
            using (ByteArrayContent content = new ByteArrayContent(chunck.ToArray()))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Wrong response");
                }

                if (!response.Headers.Contains("ETag"))
                {
                    throw new Exception("Missing ETag");
                }

                HttpHeaders headers = response.Headers;
                IEnumerable<string> values;
                string eTag = "";
                if (headers.TryGetValues("ETag", out values))
                {
                    eTag = values.First();
                }

                eTag = eTag.Replace("\"", string.Empty);
                return eTag;
            }
        }


    }
}
