namespace Domain.Entities
{
    public class StripeSettings
    {
        public string ApiKey { get; set; }
        public string WebhookSecret { get; set; }
        public Dictionary<string, StripeProduct> Products { get; set; }
    }

    public class StripeProduct
    {
        public int Quantity { get; set; }
        public string Name { get; set; }
    }
}
