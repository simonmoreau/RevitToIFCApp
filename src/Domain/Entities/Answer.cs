using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Answer : AuditableEntity
    {
        private Answer() { }
        public Answer(string id, Question question) : base()
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            if (question == null) { throw new ArgumentNullException("Question"); }
            Question = question;
            Id = id;
        }
        [Key]
        public string Id { get; private set; }
        public Question Question { get; set; }
        public string Value { get; set; }

    }
}
