namespace WebApp.Models
{
    public class UploadCompletion
    {
        public string objectKey { get; set; }
        public string uploadKey { get; set; }
        public long? size { get; set; }
        public List<string> eTags { get; set; }
    }
}
