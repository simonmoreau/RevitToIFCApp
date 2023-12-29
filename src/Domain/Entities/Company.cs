using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Company : AuditableEntity
    {
        private Company() { }

        public Company(string id) : base()
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            Sites = new List<Site>();
            Users = new List<User>();
            Id = id;
        }
        [Key]
        public string Id { get; private set; }
        public string? Name { get; set; }

        public List<Site> Sites { get; set; }
        public List<User> Users { get; set; }
    }
}
