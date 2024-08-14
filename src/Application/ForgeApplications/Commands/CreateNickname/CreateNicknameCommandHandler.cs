using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Application.Files.Queries.GetUploadUrl;
using Microsoft.Extensions.Options;

namespace Application.ForgeApplications.Commands.CreateNickname
{
    public class CreateNicknameCommandHandler : IRequestHandler<CreateNicknameCommand, string>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly ILogger<GetUploadUrlQueryHandler> _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CreateNicknameCommandHandler(DesignAutomationClient designAutomationClient, ILogger<GetUploadUrlQueryHandler> logger,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _designAutomationClient = designAutomationClient;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<string> Handle(CreateNicknameCommand request, CancellationToken cancellationToken)
        {
            NicknameRecord nicknameReccord = new NicknameRecord();
            nicknameReccord.Nickname = _forgeConfiguration.ApplicationDetail.Nickname;

            await _designAutomationClient.CreateNicknameAsync("me", nicknameReccord);

            return nicknameReccord.Nickname;
        }
    }
}
