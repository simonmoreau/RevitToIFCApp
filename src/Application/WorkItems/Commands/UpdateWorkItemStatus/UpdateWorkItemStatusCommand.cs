using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

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
