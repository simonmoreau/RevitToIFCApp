using Application.Common.Exceptions;
using Application.Common.Services;
using Application.WorkItems.Queries.GetWorkItem;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDTO>
    {
        private readonly IConversionCreditService _conversionCreditService;

        public GetUserQueryHandler(IConversionCreditService conversionCreditService)
        {
            _conversionCreditService = conversionCreditService;
        }

        public async Task<UserDTO> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            Microsoft.Graph.Models.User? user = await _conversionCreditService.GetUser(request.UserId);
            int credits = await _conversionCreditService.GetConversionCredits(request.UserId);

            UserDTO userDTO = new UserDTO()
            {
                Id = user.Id,
                Name = user.DisplayName,
                ConversionCredits = decimal.ToInt16(credits)
            };

            return userDTO;
        }
    }
}
