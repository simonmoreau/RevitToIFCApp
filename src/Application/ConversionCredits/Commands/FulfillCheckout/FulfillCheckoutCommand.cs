using Domain.Entities;
using MediatR;

namespace Application.ConversionCredits.Commands.FulfillCheckout
{
    public class FulfillCheckoutCommand : IRequest<ConversionCheckoutSession>
    {
        public FulfillCheckoutCommand(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }

    }
}
