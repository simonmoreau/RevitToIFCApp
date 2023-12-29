using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Question : AuditableEntity
    {
        private Question () { }
        public Question(string id, Plan plan, QuestionTemplate questionTemplate) : base()
        {
            if (id == null) { throw new ArgumentNullException("id"); }
            if (plan == null) { throw new ArgumentNullException("permis"); }
            if (questionTemplate == null) { throw new ArgumentNullException("questionTemplate"); }
            Plan = plan;
            Id = id;
            QuestionTemplate = questionTemplate;
        }

        [Key]
        public string Id { get; private set; }
        public Plan Plan { get; private set; }
        public string PlanId { get; set; }
        public QuestionTemplate QuestionTemplate { get; private set; }
        public string AnswerId { get; set; }
    }
}
