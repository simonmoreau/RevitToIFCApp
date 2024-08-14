namespace Domain.Entities
{
    public class ForgeConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string InputBucketKey { get; set; }
        public string OutputBucketKey { get; set; }
        public ApplicationDetail ApplicationDetail { get; set; }

    }

    public class ApplicationDetail
    {
        public string Nickname { get; set; }
        public string AppBundleName { get; set; }
        public string OutputFileName { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
    }
}
