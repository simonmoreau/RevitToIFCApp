using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public static class SavedWorkItemExtension
    {
        public static void UpdateStatus(this SavedWorkItem savedWorkItem, WorkItemStatus workItemStatus)
        {

            savedWorkItem.Status = workItemStatus.Status.ToString();
            savedWorkItem.Progress = workItemStatus.Progress;
            savedWorkItem.ReportUrl = workItemStatus.ReportUrl;
            savedWorkItem.DebugInfoUrl = workItemStatus.DebugInfoUrl;
            savedWorkItem.WorkItemId = workItemStatus.Id;

            savedWorkItem.TimeQueued = workItemStatus.Stats.TimeQueued;
            savedWorkItem.TimeDownloadStarted = workItemStatus.Stats.TimeDownloadStarted;
            savedWorkItem.TimeInstructionsStarted = workItemStatus.Stats.TimeInstructionsStarted;
            savedWorkItem.TimeInstructionsEnded = workItemStatus.Stats.TimeInstructionsEnded;
            savedWorkItem.TimeUploadEnded = workItemStatus.Stats.TimeUploadEnded;

            savedWorkItem.TimeFinished = workItemStatus.Stats.TimeFinished;
            savedWorkItem.BytesDownloaded = workItemStatus.Stats.BytesDownloaded;
            savedWorkItem.BytesUploaded = workItemStatus.Stats.BytesUploaded;
        }
    }
}
