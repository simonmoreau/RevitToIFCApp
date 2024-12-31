using Application.ForgeApplications.Commands.CreateForgeApplication;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Forge.DesignAutomation;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
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
