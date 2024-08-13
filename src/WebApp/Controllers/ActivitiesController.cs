using Application.Activities.Queries.GetActivity;
using Application.Activities.Queries.ListActivities;
using Application.ForgeApplications.Commands.CreateActivity;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Application.Sites.Queries.GetSiteList;
using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;


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
            List<string> vm = await Mediator.Send(new ListActivitiesNamesQuery());
            return vm;
        }

        [HttpPost]
        public async Task<ActionResult<Activity>> CreateActivity()
        {
            try
            {
                Activity appBundleId = await Mediator.Send(new CreateActivityCommand()
                {
                    Engine = "Autodesk.Revit+2024"
                });

                if (appBundleId == null)
                    return BadRequest();

                return CreatedAtAction(nameof(CreateForgeApplicationCommand),
                    new { id = appBundleId.Id });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating the application");
            }

        }
    }
}
