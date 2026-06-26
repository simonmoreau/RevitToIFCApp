using MediatR;

namespace Application.ConversionCredits.Commands.CreateCheckoutSession
{
    public class CreateCheckoutSessionQuery : IRequest<CheckoutSessionDTO>
    {
        internal readonly string PriceId;
        internal readonly long? Quantity;
        internal readonly string Domain;
        internal readonly string UserId;

        public CreateCheckoutSessionQuery(string priceId, long? quantity, string domain, string userId)
        {
            UserId = userId;
            PriceId = priceId;
            Quantity = quantity;
            Domain = domain;
        }
    }
}
