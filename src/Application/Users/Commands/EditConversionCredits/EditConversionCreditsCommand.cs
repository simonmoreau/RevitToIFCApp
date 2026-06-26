using MediatR;

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
