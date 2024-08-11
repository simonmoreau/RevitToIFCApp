using Application.Files.Queries.GetUploadUrl;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss;
using Autodesk.Oss.Model;
using Azure.Core;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.WorkItems.Queries.GetWorkItemStatus
{
    public class GetWorkItemStatusQueryHandler : IRequestHandler<GetWorkItemStatusQuery, WorkItemStatus>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly OssClient _ossClient;
        private readonly ILogger<GetUploadUrlQueryHandler> _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        public GetWorkItemStatusQueryHandler(DesignAutomationClient designAutomationClient, ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
            _designAutomationClient = designAutomationClient;

        }
        public async Task<WorkItemStatus> Handle(GetWorkItemStatusQuery request, CancellationToken cancellationToken)
        {
            string id = request.WorkItemId;
            WorkItemStatus status = await _designAutomationClient.GetWorkitemStatusAsync(id);

            return status;

        }
    }
}
