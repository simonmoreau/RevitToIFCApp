using Application.Activities.Queries.ListActivities;
using Application.ConversionCredits.Commands.CreateCheckoutSession;
using Application.ConversionCredits.Commands.FulfillCheckout;
using Application.ForgeApplications.Commands.CreateActivity;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using Stripe;
using Stripe.Checkout;


namespace WebApp.Controllers
{
    /// <summary>
    /// Manage activities
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ConversionCreditsController : BaseController
    {
        private readonly StripeSettings _stripeSettings;
        public ConversionCreditsController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
        }

        [HttpGet]
        [Authorize]
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

        [HttpPost]
        [Route("complete")]
        public async Task<IActionResult> Index()
        {
            string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();


            try
            {
                Stripe.Event stripeEvent = EventUtility.ConstructEvent(
                  json,
                  Request.Headers["Stripe-Signature"],
                  _stripeSettings.WebhookSecret
                );

                // If on SDK version < 46, use class Events instead of EventTypes
                if (
                  stripeEvent.Type == EventTypes.CheckoutSessionCompleted ||
                  stripeEvent.Type == EventTypes.CheckoutSessionAsyncPaymentSucceeded
                )
                {
                    Session? session = stripeEvent.Data.Object as Session;

                    ConversionCheckoutSession conversionCheckoutSession = await Mediator.Send(new FulfillCheckoutCommand(session.Id));
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest();
            }
        }
    }
}
