using Application.Common.Services;
using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

namespace Application.WorkItems.Commands.UpdateWorkItemStatus
{
    public class UpdateWorkItemStatusCommandHandler : IRequestHandler<UpdateWorkItemStatusCommand>
    {
        private readonly ISavedWorkItemService _savedWorkItemService;
        private readonly IConversionCreditService _conversionCreditService;

        public UpdateWorkItemStatusCommandHandler(ISavedWorkItemService savedWorkItemService, IConversionCreditService conversionCreditService)
        {
            _savedWorkItemService = savedWorkItemService;
            _conversionCreditService = conversionCreditService;
        }

        public async Task Handle(UpdateWorkItemStatusCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.SavedWorkItem savedWorkItem = await _savedWorkItemService.GetSavedWorkItem(request.Status.Id);

            await _savedWorkItemService.UpdateSavedWorkItemStatus(request.Status);

            if (request.Status.Status == Status.Success && !savedWorkItem.Credited)
            {
                await _conversionCreditService.EditConversionCredits(savedWorkItem.UserId, -1);
                await _savedWorkItemService.MarkSavedWorkItemAsCredited(request.Status.Id);
            }
        }
    }
}
