using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommand : IRequest<WorkItemStatus>
    {
        internal readonly string ObjectKey;
        internal readonly string ActivityId;

        public CreateWorkItemCommand(string objectKey, string activityId)
        {
            ObjectKey = objectKey;
            ActivityId = activityId;
        }
    }
}
