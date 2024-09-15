using Application.Files.Queries.GetUploadUrl;
using Application.Services;
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
        private readonly OssClient _ossClient;
        private readonly ILogger<GetUploadUrlQueryHandler> _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;
        private readonly TableServiceClient _tableServiceClient;

        public CreateWorkItemCommandHandler(DesignAutomationClient designAutomationClient,ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient, TableServiceClient tableServiceClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
            _designAutomationClient = designAutomationClient;
            _tableServiceClient = tableServiceClient;
        }

        public async Task<WorkItemStatus> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite, Scopes.DataRead });

            string inputBucketKey = _forgeConfiguration.InputBucketKey;
            string outputBucketKey = _forgeConfiguration.OutputBucketKey;

            string nickname = _forgeConfiguration.ApplicationDetail.Nickname;
            string alias = _forgeConfiguration.ApplicationDetail.Alias;

            string activityName = $"{_forgeConfiguration.ApplicationDetail.AppBundleName}Activity{request.RevitVersion}";

            string activityId = string.Format("{0}.{1}+{2}", nickname, activityName, alias);

            string objectKey = request.ObjectKey;

            Signeds3downloadResponse signedDownloadUrl = await _ossClient.SignedS3DownloadAsync(
                twoLeggedToken.AccessToken, inputBucketKey, objectKey + ".rvt");

            // prepare workitem arguments
            // 1. input file
            XrefTreeArgument inputFileArgument = new XrefTreeArgument()
            {
                Url = signedDownloadUrl.Url
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

            // prepare & submit workitem
            WorkItem workItemSpec = new WorkItem()
            {
                ActivityId = activityId,
                Arguments = new Dictionary<string, IArgument>()
                {
                    { "inputFile",  inputFileArgument },
                    { "inputJson",  inputJsonArgument },
                    { "outputFile",  outputFileArgument }
                }
            };

            WorkItemStatus workItemStatus = await _designAutomationClient.CreateWorkItemAsync(workItemSpec);

            SavedWorkItem savedWorkItem = new SavedWorkItem();
            savedWorkItem.PartitionKey = "workItems";
            savedWorkItem.RowKey = workItemStatus.Id;
            savedWorkItem.UserId = request.UserId;
            savedWorkItem.Version = request.RevitVersion;
            savedWorkItem.UpdateStatus(workItemStatus);

            TableClient tableClient = _tableServiceClient.GetTableClient("workItems");
            tableClient.CreateIfNotExists();

            Azure.Response response = await tableClient.AddEntityAsync<SavedWorkItem>(savedWorkItem);

            if (response.IsError)
            {
                throw new Exception(response.ReasonPhrase);
            }

            return workItemStatus;

        }

    }
}
