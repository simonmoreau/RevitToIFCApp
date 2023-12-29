using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class QuestionTemplate
    {
        private QuestionTemplate() { }
        public QuestionTemplate(string id) 
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            Id = id;
        }
        [Key]
        public string Id { get; private set; }
        public string? Text { get; set; }
        public Enums.Type Type { get; set; }
        public string? Groupe { get; set; }
        public Role RoleResponsible { get; set; }
    }
}
