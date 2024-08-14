using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommand : IRequest<WorkItemStatus>
    {
        internal readonly string ObjectKey;
        internal readonly string ActivityId;
        internal readonly ConversionProperties ConversionProperties;

        public CreateWorkItemCommand(WorkItemCreation workItemCreation)
        {
            ObjectKey = workItemCreation.objectKey;
            ActivityId = workItemCreation.activityId;
            ConversionProperties = workItemCreation.conversionProperties;

        }
    }
}
