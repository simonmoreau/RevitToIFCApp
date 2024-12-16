using Application.Common.Services;
using Application.Files.Queries.GetUploadUrl;
using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss;
using Autodesk.Oss.Model;
using Azure.Data.Tables;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, WorkItemStatus>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly ISavedWorkItemService _savedWorkItemService;
        private readonly OssClient _ossClient;
        private readonly ILogger<GetUploadUrlQueryHandler> _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CreateWorkItemCommandHandler(DesignAutomationClient designAutomationClient,ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient, ISavedWorkItemService savedWorkItemService)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
            _designAutomationClient = designAutomationClient;
            _savedWorkItemService = savedWorkItemService;
        }

        public async Task<WorkItemStatus> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite, Scopes.DataRead });

            string inputBucketKey = _forgeConfiguration.InputBucketKey;
            string outputBucketKey = _forgeConfiguration.OutputBucketKey;

            string nickname = _forgeConfiguration.ApplicationDetail.Nickname;
            string alias = _forgeConfiguration.ApplicationDetail.Alias;

            string activityName = BuildActiviyName(request.RevitVersion);

            string activityId = string.Format("{0}.{1}+{2}", nickname, activityName, alias);

            string objectKey = request.ObjectKey;

            // prepare workitem arguments
            // 1. input file
            XrefTreeArgument inputFileArgument = new XrefTreeArgument()
            {
                Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", inputBucketKey, objectKey + ".rvt"),
                Verb = Verb.Get,
                Headers = new Dictionary<string, string>()
                {
                    {"Authorization", "Bearer " + twoLeggedToken.AccessToken }
                }
            };

            // 2. input json
            string propertiesJson = JsonSerializer.Serialize(request.ConversionProperties);
            XrefTreeArgument inputJsonArgument = new XrefTreeArgument()
            {
                Url = "data:application/json, " + (propertiesJson.Replace("\"", "'"))
            };

            // 3. output file
            XrefTreeArgument outputFileArgument = new XrefTreeArgument()
            {
                Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", outputBucketKey, objectKey + ".ifc"),
                Verb = Verb.Put,
                Headers = new Dictionary<string, string>()
                {
                    {"Authorization", "Bearer " + twoLeggedToken.AccessToken }
                }
            };

            // 3. onComplete callback
            XrefTreeArgument onCompleteArgument = new XrefTreeArgument()
            {
                Url = $"{_forgeConfiguration.CallbackUrl}/callback/oncomplete",
                Verb = Verb.Post
            };

            // prepare & submit workitem
            WorkItem workItemSpec = new WorkItem()
            {
                ActivityId = activityId,
                Arguments = new Dictionary<string, IArgument>()
                {
                    { "inputFile",  inputFileArgument },
                    { "inputJson",  inputJsonArgument },
                    { "outputFile",  outputFileArgument },
                    { "onComplete",  onCompleteArgument }
                }
            };

            WorkItemStatus workItemStatus = await _designAutomationClient.CreateWorkItemAsync(workItemSpec);

            await _savedWorkItemService.CreateSavedWorkItemStatus(workItemStatus, request.UserId, request.RevitVersion, request.ObjectKey, request.ConversionProperties.FileName);

            return workItemStatus;

        }

        private string BuildActiviyName(string revitVersion)
        {
            int revitVersionNum = 0;
                
            if (!Int32.TryParse(revitVersion, out revitVersionNum))
            {
                throw new Exception("Revit version not found");
            }

            if (revitVersionNum < 2022)
            {
                revitVersionNum = 2022;
            }

            string activityName = $"{_forgeConfiguration.ApplicationDetail.AppBundleName}Activity{revitVersionNum.ToString()}";

            return activityName;
        }

    }
}
