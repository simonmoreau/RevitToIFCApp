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
        [Queue("orders", Connection = "StorageConnectionString")] IAsyncCollector<Order> orderQueue,
        ILogger log)
    {
      try
      {
        log.LogInformation("C# HTTP trigger function processed the checkoutSession request.");

        StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("stripe_api_key");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        CheckoutBody checkoutBody = JsonConvert.DeserializeObject<CheckoutBody>(requestBody);

        SessionLineItemOptions product = new SessionLineItemOptions
        {
          Price = checkoutBody.productId,
          Quantity = 1,
        };

        string localUri = Environment.GetEnvironmentVariable("local_uri");

        SessionCreateOptions options = new SessionCreateOptions
        {
          PaymentMethodTypes = new List<string> { "card", },
          LineItems = new List<SessionLineItemOptions> { product },
          Mode = "payment",
          SuccessUrl = localUri + "checkout/success?session_id={CHECKOUT_SESSION_ID}",
          CancelUrl = localUri + "checkout/cancel",
        };

        SessionService service = new SessionService();
        Session session = service.Create(options);

        return new OkObjectResult(new { id = session.Id });
      }
      catch (Exception ex)
      {
        return new BadRequestObjectResult(ex);
      }

    }
  }

  public class CheckoutBody
  {
    public string userId { get; set; }
    public string productId { get; set; }
  }
}
