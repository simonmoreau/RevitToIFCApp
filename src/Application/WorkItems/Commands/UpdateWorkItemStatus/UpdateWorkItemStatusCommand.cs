using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.WorkItems.Commands.UpdateWorkItemStatus
{
    public class UpdateWorkItemStatusCommand : IRequest
    {
        internal readonly WorkItemStatus Status;

        public UpdateWorkItemStatusCommand(WorkItemStatus status)
        {
            Status = status;
        }
    }
}
