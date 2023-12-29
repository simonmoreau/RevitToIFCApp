using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Objet : AuditableEntity
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Station_Service { get; set; }
        public DateTime? Travaux { get; set; }
    }
}
