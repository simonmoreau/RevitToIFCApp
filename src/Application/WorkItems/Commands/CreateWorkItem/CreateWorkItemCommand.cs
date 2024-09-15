using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Graph.Models;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommand : IRequest<WorkItemStatus>
    {
        internal readonly string ObjectKey;
        internal readonly string RevitVersion;
        internal readonly ConversionProperties ConversionProperties;
        internal readonly string UserId;

        public CreateWorkItemCommand(WorkItemCreation workItemCreation, string userId)
        {
            ObjectKey = workItemCreation.objectKey;
            RevitVersion = workItemCreation.revitVersion;
            ConversionProperties = workItemCreation.conversionProperties;
            UserId = userId;
        }
    }
}
