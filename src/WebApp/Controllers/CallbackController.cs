using Application.Files.Commands.CompleteUpload;
using Application.Files.Commands.CreateBucket;
using Application.Files.Queries.GetDownloadUrlQuery;
using Application.Files.Queries.GetFiles;
using Application.Files.Queries.GetUploadUrl;
using Application.WorkItems.Commands.CreateWorkItem;
using Application.WorkItems.Commands.UpdateWorkItemStatus;
using Application.WorkItems.Queries.GetWorkItem;
using Application.WorkItems.Queries.GetWorkItemStatus;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss.Model;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using WebApp.Models;
using Status = Autodesk.Forge.DesignAutomation.Model.Status;


namespace WebApp.Controllers
{
    /// <summary>
    /// Manage files
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CallbackController : BaseController
    {
        /// <summary>
        /// The onComplete callback controler
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("oncomplete")]
        public async Task OnComplete(WorkItemStatusDTO workItemStatusBody)
        {
            WorkItemStatus workItemStatus = new WorkItemStatus();

            workItemStatus.Status = WorkItemStatusDTO.ToEnum<Status>(workItemStatusBody.Status);
            workItemStatus.Stats = workItemStatusBody.Stats;
            workItemStatus.ReportUrl = workItemStatusBody.ReportUrl;
            workItemStatus.Progress = workItemStatusBody.Progress;
            workItemStatus.Id = workItemStatusBody.Id;

            await Mediator.Send(new UpdateWorkItemStatusCommand(workItemStatus));
        }
    }
}
