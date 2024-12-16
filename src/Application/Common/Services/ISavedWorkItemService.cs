using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;

namespace Application.Common.Services
{
    public interface ISavedWorkItemService
    {
        Task CreateSavedWorkItemStatus(WorkItemStatus workItemStatus, string userId, string revitVersion, string objectKey, string fileName);
        List<SavedWorkItem> GetSavedWorkItems(string userId);
        Task UpdateSavedWorkItemStatus(WorkItemStatus workItemStatus);
    }
}