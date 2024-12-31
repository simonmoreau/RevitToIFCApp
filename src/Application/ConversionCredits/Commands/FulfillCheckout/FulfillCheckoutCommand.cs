using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ConversionCredits.Commands.FulfillCheckout
{
    public class FulfillCheckoutCommand : IRequest<string>
    {
        public FulfillCheckoutCommand(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }

    }
}
