using Autodesk.Forge.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Text;
using WebClient.Models;

namespace WebClient.Services
{
    public class UploadService : IUploadService
    {
        private readonly HttpClient _httpClient;

        public UploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UploadChunk(MemoryStream chunck, string url)
        {
            using (var content = new ByteArrayContent(chunck.ToArray()))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Wrong response");
                }
            }
        }


    }
}
