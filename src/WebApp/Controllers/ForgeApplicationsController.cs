using Application.ForgeApplications.Queries.ListForgeApplications;
using Application.ForgeApplications.Commands.CreateNickname;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Mvc;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Application.ForgeApplications.Commands.CreateActivity;


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

        /// <summary>
        /// Update nickname
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("nickname")]
        public async Task<ActionResult<string>> UpdateNickname()
        {
            string vm = await Mediator.Send(new CreateNicknameCommand());
            return vm;
        }

        [HttpPost]
        [Route("appbundle")]
        public async Task<ActionResult<AppBundle>> CreateApplication()
        {
            try
            {
                AppBundle appBundleId = await Mediator.Send(new CreateForgeApplicationCommand()
                {
                    RevitVersion = "2024"
                });

                if (appBundleId == null)
                    return BadRequest();

                return appBundleId;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating the application");
            }

        }

        [HttpPost]
        [Route("applications")]
        public async Task<ActionResult<List<string>>> CreateFullApplication()
        {
            try
            {
                string[] revitVersions = ["2022", "2023", "2024", "2025"];

                List<string> creationIds = new List<string>();
                foreach (string version in revitVersions)
                {
                    creationIds.Add(await CreateApplication(version));
                }

                return creationIds;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating the application");
            }

        }

        private async Task<string> CreateApplication(string version)
        {
            AppBundle appBundle = await Mediator.Send(new CreateForgeApplicationCommand()
            {
                RevitVersion = version
            });

            Activity activity = await Mediator.Send(new CreateActivityCommand()
            {
                RevitVersion = version
            });

            return activity.Id;
        }
    }
}
