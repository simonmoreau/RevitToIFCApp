using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : AuditableEntity
    {
        private User () { }
        public User(string id, Company company) : base()
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            if (company == null) { throw new ArgumentNullException("company"); }
            Id = id;
            Company = company;
            CompanyId = company.Id;
        }
        [Key]
        public string Id { get; private set; }
        public Company Company { get; private set; }
        public string CompanyId { get; private set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
