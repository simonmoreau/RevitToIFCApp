using Application.Activities.Queries.ListActivities;
using Application.ConversionCredits.Commands.CreateCheckoutSession;
using Application.ConversionCredits.Commands.FulfillCheckout;
using Application.ForgeApplications.Commands.CreateActivity;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;


namespace WebApp.Controllers
{
    /// <summary>
    /// Manage activities
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ConversionCreditsController : BaseController
    {
        [HttpGet]
        [Route("checkout")]
        public async Task<CheckoutSessionDTO> CreateCheckoutSession(string price, string domain)
        {
            string? userId = User.GetObjectId();
            CheckoutSessionDTO checkoutSession = await Mediator.Send(new CreateCheckoutSessionQuery(price, 1, domain, userId));

            return checkoutSession;

        }

        [HttpPost]
        [Route("fulfill")]
        public async Task<ActionResult<ConversionCheckoutSession>> FulfillCheckoutSession([FromBody] string sessionId)
        {
            ConversionCheckoutSession conversionCheckoutSession = await Mediator.Send(new FulfillCheckoutCommand(sessionId));

            return Ok(conversionCheckoutSession);
        }
    }
}
