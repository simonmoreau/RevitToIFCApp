using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommand : IRequest<WorkItemStatus>
    {
        internal readonly string ObjectKey;
        internal readonly string ActivityId;
        internal readonly string ConversionProperties;

        public CreateWorkItemCommand(string objectKey, string activityId)
        {
            ObjectKey = objectKey;
            ActivityId = activityId;
            ConversionProperties = "";

        }
    }
}
