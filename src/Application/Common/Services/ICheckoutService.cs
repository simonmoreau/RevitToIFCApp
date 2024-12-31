using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public interface ICheckoutService
    {
        Task<ConversionCheckoutSession> CreateCheckoutSession(string sessionId);
        Task<ConversionCheckoutSession?> GetCheckoutSession(string checkoutSessionId);
        Task UpdateCheckoutSessionStatus(string sessionId, ConversionCheckoutStatus conversionCheckoutStatus);
    }
}
