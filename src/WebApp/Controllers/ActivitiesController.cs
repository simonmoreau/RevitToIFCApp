using Application.Activities.Queries.GetActivity;
using Application.Activities.Queries.ListActivities;
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
    public class ActivitiesController : BaseController
    {
        /// <summary>
        /// Get all Activities
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetAllActivities")]
        public async Task<IEnumerable<string>> GetAllActivities()
        {
            var vm = await Mediator.Send(new ListActivitiesNamesQuery());
            return vm;
        }
    }
}
