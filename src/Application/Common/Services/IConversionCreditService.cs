using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public interface IConversionCreditService
    {
        Task<Microsoft.Graph.Models.User?> GetUser(string userId);
        Task<int> GetConversionCredits(string userId);
        Task<int> EditConversionCredits(string userId, int quantity);
    }
}
