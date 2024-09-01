namespace Domain.Common
{
    public class AuditableEntity
    {
        public string? CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public AuditableEntity()
        {
            Created = DateTime.Now;
            LastModified = DateTime.Now;
        }
    }
}
