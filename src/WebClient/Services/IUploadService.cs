
namespace WebClient.Services
{
    public interface IUploadService
    {
        Task<string> UploadChunk(MemoryStream chunck, string url);
    }
}