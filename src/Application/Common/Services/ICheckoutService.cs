using Domain.Entities;

namespace Application.Common.Services
{
    public interface ICheckoutService
    {
        Task<ConversionCheckoutSession> CreateCheckoutSession(string sessionId);
        Task<ConversionCheckoutSession?> GetCheckoutSession(string checkoutSessionId);
        Task UpdateCheckoutSessionStatus(string sessionId, ConversionCheckoutStatus conversionCheckoutStatus);
    }
}
