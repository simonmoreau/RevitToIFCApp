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
        [HttpGet(Name = "GetCheckoutSession")]
        [Route("checkout")]
        public async Task<CheckoutSessionDTO> CreateCheckoutSession(string price, string domain)
        {
            CheckoutSessionDTO checkoutSession = await Mediator.Send(new CreateCheckoutSessionQuery(price, 1, domain));

            return checkoutSession;

        }
    }
}
