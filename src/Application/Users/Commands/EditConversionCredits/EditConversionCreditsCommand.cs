using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.EditConversionCredits
{
    public class EditConversionCreditsCommand : IRequest<int>
    {
        public EditConversionCreditsCommand(string userId, int quantity)
        {
            UserId = userId;
            Quantity = quantity;
        }

        public readonly string UserId;
        public readonly int Quantity;
    }
}
