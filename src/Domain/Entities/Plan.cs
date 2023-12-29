using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Plan : AuditableEntity
    {
        private Plan() { }
        public Plan(string id, ConstructionSite constructionSite) : base() 
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            if (constructionSite == null) { throw new ArgumentNullException("constructionSite"); }
            ConstructionSite = constructionSite;
            Questions = new List<Question>();
            Id = id;
            ConstructionSiteId = constructionSite.Id;
        }
        [Key]
        public string Id { get; private set; }
        public ConstructionSite ConstructionSite { get; private set; }
        public string ConstructionSiteId { get; set; }
        public string? Name { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string? Infos { get; set; }
        public List<Question> Questions { get; private set; }
    }
}
