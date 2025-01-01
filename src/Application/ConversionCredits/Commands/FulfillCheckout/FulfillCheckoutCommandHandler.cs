using Application.Files.Commands.CompleteUpload;
using MediatR;
using Microsoft.Extensions.Azure;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe.Checkout;
using Application.ConversionCredits.Commands.CreateCheckoutSession;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Application.Common.Services;


namespace Application.ConversionCredits.Commands.FulfillCheckout
{
    public class FulfillCheckoutCommandHandler : IRequestHandler<FulfillCheckoutCommand, ConversionCheckoutSession>
    {
        private readonly ILogger _logger;
        private readonly StripeSettings _stripeSettings;
        private readonly IConversionCreditService _conversionCreditService;
        private readonly ICheckoutService _checkoutService;

        public FulfillCheckoutCommandHandler(ILogger<CreateCheckoutSessionQueryHandler> logger,
            IOptions<StripeSettings> stripeSettings, IConversionCreditService conversionCreditService, ICheckoutService checkoutService)
        {
            _logger = logger;
            _stripeSettings = stripeSettings.Value;
            _conversionCreditService = conversionCreditService;
            _checkoutService = checkoutService;
        }

        public async Task<ConversionCheckoutSession> Handle(FulfillCheckoutCommand request, CancellationToken cancellationToken)
        {
            // Set your secret key. Remember to switch to your live secret key in production.
            // See your keys here: https://dashboard.stripe.com/apikeys
            StripeConfiguration.ApiKey = _stripeSettings.ApiKey;

            // TODO: Make this function safe to run multiple times,
            // even concurrently, with the same session ID
            ConversionCheckoutSession? conversionCheckoutSession = await _checkoutService.GetCheckoutSession(request.SessionId);
            if (conversionCheckoutSession != null)
            {
                return conversionCheckoutSession;
            }
            else
            {
                conversionCheckoutSession = await _checkoutService.CreateCheckoutSession(request.SessionId);
            }

            // TODO: Make sure fulfillment hasn't already been
            // peformed for this Checkout Session


            // Retrieve the Checkout Session from the API with line_items expanded
            SessionGetOptions options = new SessionGetOptions
            {
                Expand = new List<string> { "line_items" },
            };

            SessionService service = new SessionService();
            Session checkoutSession = service.Get(request.SessionId, options);

            // Check the Checkout Session's payment_status property
            // to determine if fulfillment should be peformed
            if (checkoutSession.PaymentStatus != "unpaid")
            {
                string? userId = checkoutSession.Metadata.GetValueOrDefault<string, string>("user_id");
                if (userId == null)
                {
                    return await FailedFullfilment(request, conversionCheckoutSession);
                }

                // TODO: Perform fulfillment of the line items
                string? productId = checkoutSession.LineItems.FirstOrDefault()?.Price.Id;
                if (productId == null)
                {
                    return await FailedFullfilment(request, conversionCheckoutSession);
                }

                if (!_stripeSettings.Products.ContainsKey(productId))
                {
                    return await FailedFullfilment(request, conversionCheckoutSession);
                }

                StripeProduct stripeProduct = _stripeSettings.Products[productId];

                await _conversionCreditService.EditConversionCredits(userId, stripeProduct.Quantity);

                // TODO: Record/save fulfillment status for this
                // Checkout Session
                await _checkoutService.UpdateCheckoutSessionStatus(request.SessionId,ConversionCheckoutStatus.Complete);
                conversionCheckoutSession.Status = ConversionCheckoutStatus.Complete;
            }

            return conversionCheckoutSession;
        }

        private async Task<ConversionCheckoutSession> FailedFullfilment(FulfillCheckoutCommand request, ConversionCheckoutSession? conversionCheckoutSession)
        {
            await _checkoutService.UpdateCheckoutSessionStatus(request.SessionId, ConversionCheckoutStatus.Failed);
            conversionCheckoutSession.Status = ConversionCheckoutStatus.Failed;
            return conversionCheckoutSession;
        }
    }
}
