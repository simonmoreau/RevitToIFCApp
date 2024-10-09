using Application.Common.Services;
using Application.Files.Queries.GetUploadUrl;
using Autodesk.Authentication;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss;
using Azure.Data.Tables;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.WorkItems.Queries.GetWorkItemStatus
{
    public class GetWorkItemStatusQueryHandler : IRequestHandler<GetWorkItemStatusQuery, WorkItemStatus>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly ISavedWorkItemService _savedWorkItemService;
        private readonly OssClient _ossClient;
        private readonly ILogger<GetUploadUrlQueryHandler> _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        public GetWorkItemStatusQueryHandler(DesignAutomationClient designAutomationClient, ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient, ISavedWorkItemService savedWorkItemService)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
            _designAutomationClient = designAutomationClient;
            _savedWorkItemService = savedWorkItemService;


        }
        public async Task<WorkItemStatus> Handle(GetWorkItemStatusQuery request, CancellationToken cancellationToken)
        {
            string id = request.WorkItemId;
            WorkItemStatus workItemStatus = await _designAutomationClient.GetWorkitemStatusAsync(id);

            await _savedWorkItemService.UpdateSavedWorkItemStatus(workItemStatus);


            return workItemStatus;

        }
    }
}
