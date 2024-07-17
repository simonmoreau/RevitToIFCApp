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

namespace Application.Files.Queries
{
    public class GetUploadUrlQueryHandler : IRequestHandler<GetUploadUrlQuery, GetUploadUrlVm>
    {
        private readonly IObjectsApi _objectsApi;
        private readonly ILogger _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly IConfiguration _configuration;
        private readonly ForgeConfiguration _forgeConfiguration;

        private readonly string _accessTokenExpiredMessage = "Access token provided is invalid or expired.";
        private readonly string _forbiddenMessage = "403 (Forbidden)";

        public GetUploadUrlQueryHandler(IObjectsApi objectsApi, ILogger<GetUploadUrlQueryHandler> logger, AuthenticationClient authenticationClient, 
            IConfiguration configuration, IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _objectsApi = objectsApi;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _configuration = configuration;
            _forgeConfiguration = forgeConfiguration.Value;
            
        }

        public async Task<GetUploadUrlVm> Handle(GetUploadUrlQuery request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite });

            string key = _forgeConfiguration.BucketKey;

            (Signeds3uploadResponse, string) test = await GetUploadUrlsWithRetry(key, "test", 10, 0, "test", twoLeggedToken.AccessToken, "data:write", "testRequest");

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
                    Autodesk.Forge.Core.ApiResponse<Signeds3uploadResponse> response = await _objectsApi.SignedS3UploadAsync(
                          bucketKey: bucketKey,
                          objectKey: objectKey,
                          parts: parts,
                          firstPart: firstPart,
                          uploadKey: uploadKey,
                          accessToken: accessToken,
                          xAdsAcmScopes: projectScope);

                    return (response.Content, accessToken);
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


    }

    static class Constants
    {
        public const int MaxRetry = 5;
        public const ulong ChunkSize = 5 * 1024 * 1024;
        public const int BatchSize = 25;
    }
}
