using Application.Common.Services;
using Application.Files.Queries.GetUploadUrl;
using Application.WorkItems.Queries.GetWorkItemStatus;
using Autodesk.Authentication;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Domain.Entities.SavedWorkItem savedWorkItemStatus = await _savedWorkItemService.GetSavedWorkItem(request.Status.Id);

            await _savedWorkItemService.UpdateSavedWorkItemStatus(request.Status);

            if (request.Status.Status == Status.Success)
            {
                await _conversionCreditService.EditConversionCredits(savedWorkItemStatus.UserId, -1);
            }

            
        }
    }
}
