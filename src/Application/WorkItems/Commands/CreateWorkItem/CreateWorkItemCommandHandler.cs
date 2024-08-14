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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, WorkItemStatus>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly OssClient _ossClient;
        private readonly ILogger<GetUploadUrlQueryHandler> _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CreateWorkItemCommandHandler(DesignAutomationClient designAutomationClient,ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
            _designAutomationClient = designAutomationClient;
        }

        public async Task<WorkItemStatus> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite, Scopes.DataRead });

            string inputBucketKey = _forgeConfiguration.InputBucketKey;
            string outputBucketKey = _forgeConfiguration.OutputBucketKey;

            string objectKey = request.ObjectKey;

            Signeds3downloadResponse signedDownloadUrl = await _ossClient.SignedS3DownloadAsync(
                twoLeggedToken.AccessToken,inputBucketKey, objectKey + ".rvt");

            Signeds3uploadResponse signedUploadUrl = await _ossClient.SignedS3UploadAsync(
                twoLeggedToken.AccessToken,outputBucketKey, objectKey + ".ifc");

            WorkItem workItem = new WorkItem()
            {
                ActivityId = request.ActivityId,
                Arguments = new Dictionary<string, IArgument>
                    {
                        { "inputFile",  new XrefTreeArgument() 
                            { Url = signedDownloadUrl.Url, Verb = Verb.Get } },
                        { "outputFile", new XrefTreeArgument 
                        { Verb=Verb.Put, Headers = new Dictionary<string, string>() { { "Content-Type", "binary/octet-stream" } }, Url = signedUploadUrl.Urls.First() } }
                    }
            };


            Autodesk.Forge.Core.ApiResponse<WorkItemStatus> createWorkItemResponse = await _designAutomationClient.WorkItemsApi.CreateWorkItemAsync(workItem);

            return createWorkItemResponse.Content;
        }
    }
}
