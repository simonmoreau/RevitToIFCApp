using Application.WorkItems.Commands.UpdateWorkItemStatus;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Mvc;
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
