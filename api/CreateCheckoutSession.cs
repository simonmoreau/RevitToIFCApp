using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;

namespace api
{
    public static class CreateCheckoutSession
    {
        [FunctionName("CreateCheckoutSession")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "checkoutSession")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed the checkoutSession request.");

            StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("stripe_api_key");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            SessionLineItemOptions product = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = 2000,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "T-shirt",
                    },

                },
                Quantity = 1,
            };

            SessionCreateOptions options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", },
                LineItems = new List<SessionLineItemOptions> { product },
                Mode = "payment",
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
            };

            SessionService service = new SessionService();
            Session session = service.Create(options);

            return new OkObjectResult(new { id = session.Id });
        }
    }
}
