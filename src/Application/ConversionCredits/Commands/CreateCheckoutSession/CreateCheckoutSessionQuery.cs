using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ConversionCredits.Commands.CreateCheckoutSession
{
    public class CreateCheckoutSessionQuery : IRequest<string>
    {
        internal readonly string PriceId;
        internal readonly long? Quantity;

        public CreateCheckoutSessionQuery(string priceId, long? quantity)
        {
            PriceId = priceId;
            Quantity = quantity;
        }
    }
}
