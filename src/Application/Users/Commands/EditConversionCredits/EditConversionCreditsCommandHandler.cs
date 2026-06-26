using MediatR;
using Application.Common.Services;

namespace Application.Users.Commands.EditConversionCredits
{
    public class EditConversionCreditsCommandHandler : IRequestHandler<EditConversionCreditsCommand, int>
    {
        private readonly IConversionCreditService _conversionCreditService;

        public EditConversionCreditsCommandHandler(IConversionCreditService conversionCreditService)
        {
            _conversionCreditService = conversionCreditService;
        }

        public async Task<int> Handle(EditConversionCreditsCommand request, CancellationToken cancellationToken)
        {
            int updated = await _conversionCreditService.EditConversionCredits(request.UserId, request.Quantity);

            return updated;
        }
    }

    }
