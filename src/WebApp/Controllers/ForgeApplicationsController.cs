using Application.Activities.Queries.GetActivity;
using Application.Activities.Queries.ListActivities;
using Application.ForgeApplications.Queries.ListForgeApplications;
using Application.Sites.Queries.GetSiteList;
using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace WebApp.Controllers
{
    /// <summary>
    /// Manage activities
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ForgeApplicationsController : BaseController
    {
        /// <summary>
        /// Get all applications details
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetAllApps")]
        public async Task<ActionResult<ListForgeApplicationsVm>> GetAllApps()
        {
            ListForgeApplicationsVm vm = await Mediator.Send(new ListForgeApplicationsQuery());
            return vm;
        }
    }
}
