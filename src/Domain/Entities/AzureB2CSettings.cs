namespace Domain.Entities
{
    public class AzureB2CSettings
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string B2cExtensionAppClientId { get; set; }
        public string UsersFileName { get; set; }

    }
}
