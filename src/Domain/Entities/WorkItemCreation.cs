namespace Domain.Entities
{
    public class WorkItemCreation
    {
        public string objectKey { get; set; }
        public string activityId { get; set; }
        public ConversionProperties conversionProperties { get; set; }
    }
}
