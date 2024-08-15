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
        public async Task<ActionResult<object>> CreateApplication()
        {
            try
            {
                AppBundle appBundleId = await Mediator.Send(new CreateForgeApplicationCommand()
                {
                    RevitVersion = "Autodesk.Revit+2024",
                    AppbundleFile = @"C:\Users\smoreau\Github\RevitToIFCApp\src\Bundle\bin\Debug\RevitToIFCBundle.zip"
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

        [HttpPost]
        [Route("applications")]
        public async Task<ActionResult<object>> CreateFullApplication()
        {
            try
            {
                string[] revitVersions = ["2022", "2023", "2024", "2025"];

                List<Task<string>> creationTasks = new List<Task<string>>();
                foreach (string version in revitVersions)
                {
                    creationTasks.Add(CreateApplication(version));
                }

                string[] activiyIds = await Task.WhenAll(creationTasks);

                return CreatedAtAction(nameof(CreateForgeApplicationCommand),new { ids = activiyIds });
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
                RevitVersion = version,
                AppbundleFile = @"C:\Users\smoreau\Github\RevitToIFCApp\src\Bundle\bin\Debug\RevitToIFCBundle.zip"
            });

            Activity activity = await Mediator.Send(new CreateActivityCommand()
            {
                RevitVersion = "2024"
            });

            return activity.Id;
        }
    }
}
