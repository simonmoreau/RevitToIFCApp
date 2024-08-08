
namespace WebClient.Services
{
    public interface IUploadService
    {
        Task UploadChunk(MemoryStream chunck, string url);
    }
}