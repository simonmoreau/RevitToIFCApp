using MediatR;
using Autodesk.Oss.Model;
using Autodesk.Oss;
using Microsoft.Extensions.Logging;
using Autodesk.Authentication.Model;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Autodesk.Authentication;
using System.Text.Json;
using Stripe.Checkout;
using Stripe;

namespace Application.ConversionCredits.Commands.CreateCheckoutSession
{
    public class CreateCheckoutSessionQueryHandler : IRequestHandler<CreateCheckoutSessionQuery, string>
    {
        private readonly ILogger _logger;
        private readonly StripeSettings _stripeSettings;


        public CreateCheckoutSessionQueryHandler(ILogger<CreateCheckoutSessionQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<StripeSettings> stripeSettings, OssClient ossClient)
        {
            _logger = logger;
            _stripeSettings = stripeSettings.Value;
        }

        public async Task<string> Handle(CreateCheckoutSessionQuery request, CancellationToken cancellationToken)
        {
            StripeConfiguration.ApiKey = _stripeSettings.ApiKey;
            string domain = "http://localhost:4242";

            SessionCreateOptions options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = request.PriceId,
                    Quantity = request.Quantity,
                  },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success.html",
                CancelUrl = domain + "/cancel.html",
            };
            SessionService service = new SessionService();
            Session session = service.Create(options);

            return session.Url;
        }
    }
}
