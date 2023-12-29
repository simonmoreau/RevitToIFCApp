using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Site : AuditableEntity
    {
        private Site () { }
        public Site(string id, Company company) : base()
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            if (company == null) { throw new ArgumentNullException("company"); }
            Company = company;
            ConstructionSites = new List<ConstructionSite>();
            Id = id;
            CompanyId = company.Id;
        }
        [Key]
        public string Id { get; private set; }
        public Company Company { get; private set; }
        public string CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Localisation { get; set; }
        public List<ConstructionSite> ConstructionSites { get; set; }

    }
}
