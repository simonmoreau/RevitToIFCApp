using Autodesk.Forge.DesignAutomation.Model;

namespace Application.Common.Services
{
    public interface ISavedWorkItemService
    {
        Task CreateSavedWorkItemStatus(WorkItemStatus workItemStatus, string userId, string revitVersion);
        Task UpdateSavedWorkItemStatus(WorkItemStatus workItemStatus);
    }
}