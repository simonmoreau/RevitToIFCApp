using Application.ForgeApplications.Queries.ListForgeApplications;
using Application.ForgeApplications.Commands.CreateNickname;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Mvc;
using Application.ForgeApplications.Commands.CreateForgeApplication;


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
                    Engine = "Autodesk.Revit+2024",
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
    }
}
