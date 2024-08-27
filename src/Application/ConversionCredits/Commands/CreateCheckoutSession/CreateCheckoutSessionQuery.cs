using MediatR;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ConversionCredits.Commands.CreateCheckoutSession
{
    public class CreateCheckoutSessionQuery : IRequest<CheckoutSessionDTO>
    {
        internal readonly string PriceId;
        internal readonly long? Quantity;
        internal readonly string Domain;

        public CreateCheckoutSessionQuery(string priceId, long? quantity, string domain)
        {
            PriceId = priceId;
            Quantity = quantity;
            Domain = domain;
        }
    }
}
