using Application.Common.Services;
using Autodesk.Authentication.Model;
using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.WorkItems.Commands.CreateWorkItem
{
    public class CreateWorkItemCommandHandler : IRequestHandler<CreateWorkItemCommand, WorkItemStatus>
    {
        private readonly IAutodeskPlatformService _autodeskPlatformService;
        private readonly ISavedWorkItemService _savedWorkItemService;
        private readonly IConversionCreditService _conversionCreditService;
        private readonly ILogger<CreateWorkItemCommandHandler> _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CreateWorkItemCommandHandler(IAutodeskPlatformService autodeskPlatformService, ILogger<CreateWorkItemCommandHandler> logger,
            IOptions<ForgeConfiguration> forgeConfiguration,
            ISavedWorkItemService savedWorkItemService, IConversionCreditService conversionCreditService)
        {
            _autodeskPlatformService = autodeskPlatformService;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
            _savedWorkItemService = savedWorkItemService;
            _conversionCreditService = conversionCreditService;
        }

        public async Task<WorkItemStatus> Handle(CreateWorkItemCommand request, CancellationToken cancellationToken)
        {
            int credits = await _conversionCreditService.GetConversionCredits(request.UserId);

            if (credits == 0)
            {
                return new WorkItemStatus()
                {
                    Status = Status.FailedInstructions,
                    Progress = "You don't have enought credits to convert this file."
                };
            }

            string accessToken = await _autodeskPlatformService.GetAccessTokenAsync(new List<Scopes> { Scopes.DataWrite, Scopes.DataRead });

            string inputBucketKey = _forgeConfiguration.InputBucketKey;
            string outputBucketKey = _forgeConfiguration.OutputBucketKey;

            string nickname = _forgeConfiguration.ApplicationDetail.Nickname;
            string alias = _forgeConfiguration.ApplicationDetail.Alias;

            string activityName = BuildActiviyName(request.RevitVersion);

            string activityId = string.Format("{0}.{1}+{2}", nickname, activityName, alias);

            string objectKey = request.ObjectKey;

            XrefTreeArgument inputFileArgument = new XrefTreeArgument()
            {
                Url = $"urn:adsk.objects:os.object:{inputBucketKey}/{objectKey}.rvt",
                Verb = Verb.Get,
                Headers = new Dictionary<string, string>()
                {
                    {"Authorization", "Bearer " + accessToken }
                }
            };

            string propertiesJson = JsonSerializer.Serialize(request.ConversionProperties);
            XrefTreeArgument inputJsonArgument = new XrefTreeArgument()
            {
                Url = "data:application/json, " + (propertiesJson.Replace("\"", "'"))
            };

            XrefTreeArgument outputFileArgument = new XrefTreeArgument()
            {
                Url = $"urn:adsk.objects:os.object:{outputBucketKey}/{objectKey}.ifc",
                Verb = Verb.Put,
                Headers = new Dictionary<string, string>()
                {
                    {"Authorization", "Bearer " + accessToken }
                }
            };

            XrefTreeArgument onCompleteArgument = new XrefTreeArgument()
            {
                Url = $"{_forgeConfiguration.CallbackUrl}/callback/oncomplete",
                Verb = Verb.Post
            };

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

            WorkItemStatus workItemStatus = await _autodeskPlatformService.CreateWorkItemAsync(workItemSpec);

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
