using Application.Sites.Queries.GetSiteList;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Oss.Http;
using Autodesk.Oss.Model;
using Autodesk.Oss;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Logging;
using Autodesk.SDKManager;
using System.Text.RegularExpressions;
using Autodesk.Authentication.Model;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Autodesk.Authentication;

namespace Application.Files.Queries.GetUploadUrl
{
    public class GetUploadUrlQueryHandler : IRequestHandler<GetUploadUrlQuery, GetUploadUrlVm>
    {
        private readonly OssClient _ossClient;
        private readonly ILogger _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        private readonly string _accessTokenExpiredMessage = "Access token provided is invalid or expired.";
        private readonly string _forbiddenMessage = "403 (Forbidden)";

        public GetUploadUrlQueryHandler(ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient, 
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<GetUploadUrlVm> Handle(GetUploadUrlQuery request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite });

            string bucketKey = _forgeConfiguration.BucketKey;
            string objectKey = "file";
            string projectScope = "data:write";
            string requestIdPrefix = "";
            string uploadKey = null;

            string requestId = HandleRequestId(requestIdPrefix, bucketKey, objectKey);

            (Signeds3uploadResponse, string) test = await GetUploadUrlsWithRetry(
                bucketKey, objectKey, 10, 0, uploadKey,
                twoLeggedToken.AccessToken, projectScope, requestId);

            GetUploadUrlVm uploadUrls = new GetUploadUrlVm();

            uploadUrls.Urls = test.Item1.Urls;

            return uploadUrls;
        }

        private async Task<(Signeds3uploadResponse, string)> GetUploadUrlsWithRetry(string bucketKey, string objectKey, int numberOfChunks, int chunksUploaded, string uploadKey, string accessToken, string projectScope, string requestId)
        {
            int attemptCount = 0;
            int parts = Math.Min(numberOfChunks - chunksUploaded, Constants.BatchSize);
            int firstPart = chunksUploaded + 1;

            do
            {
                _logger.LogInformation("{requestId} Refreshing URL attempt:{attemptCount}.", requestId, attemptCount);

                try
                {
                    Signeds3uploadResponse response = await _ossClient.SignedS3UploadAsync(
                          bucketKey: bucketKey,
                          objectKey: objectKey,
                          parts: parts,
                          firstPart: firstPart,
                          uploadKey: uploadKey,
                          accessToken: accessToken,
                          xAdsAcmScopes: projectScope);

                    return (response, accessToken);
                }
                catch (OssApiException e)
                {
                    if (e.Message.Contains(_accessTokenExpiredMessage))
                    {
                        attemptCount++;

                        accessToken = "_authentication.GetUpdatedAccessToken()";
                        _logger.LogInformation("{requestId} Token expired. Trying to refresh", requestId);
                    }
                    else
                    {
                        _logger.LogWarning("{requestId} Error: {errorMessage}", requestId, e.Message);
                        throw;
                    }
                }
                //} while (attemptCount < _maxRetryOnTokenExpiry);
            } while (attemptCount < 10);

            throw new OssApiException($"{requestId} Error: Fail getting upload urls after maximum retry");
        }

        private string HandleRequestId(string parentRequestId, string bucketKey, string objectKey)
        {
            var requestId = !string.IsNullOrEmpty(parentRequestId) ? parentRequestId : Guid.NewGuid().ToString();
            requestId = requestId + ":" + GenerateSdkRequestId(bucketKey, objectKey);
            //_forgeService.Client.DefaultRequestHeaders.Add("x-ads-request-id", requestId);
            return requestId;
        }

        private string GenerateSdkRequestId(string bucketKey, string objectKey)
        {
            return bucketKey + "/" + objectKey;
        }

    }

    static class Constants
    {
        public const int MaxRetry = 5;
        public const ulong ChunkSize = 5 * 1024 * 1024;
        public const int BatchSize = 25;
    }
}
