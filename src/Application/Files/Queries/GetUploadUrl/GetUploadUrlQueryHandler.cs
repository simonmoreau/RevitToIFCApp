﻿using MediatR;
using Autodesk.Oss.Model;
using Autodesk.Oss;
using Microsoft.Extensions.Logging;
using Autodesk.Authentication.Model;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Autodesk.Authentication;

namespace Application.Files.Queries.GetUploadUrl
{
    public class GetUploadUrlQueryHandler : IRequestHandler<GetUploadUrlQuery, Signeds3uploadResponse>
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

        public async Task<Signeds3uploadResponse> Handle(GetUploadUrlQuery request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite });

            string bucketKey = _forgeConfiguration.InputBucketKey;
            string objectKey = request.ObjectKey + ".rvt";
            string projectScope = "data:write";
            string requestIdPrefix = "";
            string uploadKey = null;

            string requestId = HandleRequestId(requestIdPrefix, bucketKey, objectKey);

            Signeds3uploadResponse signedUrlResponse = await GetUploadUrlsWithRetry(
                bucketKey, objectKey, request.ChunksNumber, uploadKey,
                twoLeggedToken.AccessToken, projectScope, requestId);

            
            return signedUrlResponse;
        }

        private async Task<Signeds3uploadResponse> GetUploadUrlsWithRetry(string bucketKey, string objectKey, int numberOfChunks, string uploadKey, string accessToken, string projectScope, string requestId)
        {
            int attemptCount = 0;

            do
            {
                _logger.LogInformation("{requestId} Refreshing URL attempt:{attemptCount}.", requestId, attemptCount);

                try
                {
                    Signeds3uploadResponse response = await _ossClient.SignedS3UploadAsync(
                          bucketKey: bucketKey,
                          objectKey: objectKey,
                          parts: numberOfChunks,
                          uploadKey: uploadKey,
                          accessToken: accessToken,
                          minutesExpiration:15,
                          xAdsAcmScopes: projectScope);

                    return response;
                }
                catch (OssApiException e)
                {
                    _logger.LogWarning("{requestId} Error: {errorMessage}", requestId, e.Message);
                    throw;
                }
                //} while (attemptCount < _maxRetryOnTokenExpiry);
            } while (attemptCount < 10);

            throw new OssApiException($"{requestId} Error: Fail getting upload urls after maximum retry");
        }

        private string HandleRequestId(string parentRequestId, string bucketKey, string objectKey)
        {
            string requestId = !string.IsNullOrEmpty(parentRequestId) ? parentRequestId : Guid.NewGuid().ToString();
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
