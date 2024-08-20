using Application.Activities.Queries.ListActivities;
using Application.ConversionCredits.Commands.CreateCheckoutSession;
using Application.ForgeApplications.Commands.CreateActivity;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Autodesk.Forge.DesignAutomation.Model;
using Microsoft.AspNetCore.Mvc;


namespace WebApp.Controllers
{
    /// <summary>
    /// Manage activities
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConversionCreditsController : BaseController
    {
        [HttpPost]
        [Route("checkout")]
        public async Task<ActionResult<string>> CreateCheckoutSession()
        {
            try
            {
                string url = await Mediator.Send(new CreateCheckoutSessionQuery("price_1I1FBgFjsZIqAFNU6psC3AjP", 1));

                if (url == null)
                    return BadRequest();

                return url;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating the application");
            }

        }
    }
}
