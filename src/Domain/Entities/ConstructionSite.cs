using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ConstructionSite : AuditableEntity
    {
        private ConstructionSite() { }
        public ConstructionSite(string id, Site site) : base()
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            if (site == null) { throw new ArgumentNullException("site"); }
            Site = site;
            Plans = new List<Plan>();
            Id = id;
            SiteId = Site.Id;
        }
        [Key]
        public string Id { get; private set; }
        public Site Site { get; private set; }
        public string SiteId { get; set; }
        public string? Name { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public readonly List<Plan> Plans;
    }
}
