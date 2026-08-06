using Application.Common.Services;
using Autodesk.Oss;
using Autodesk.Oss.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Files.Queries.GetUploadUrl
{
    public class GetUploadUrlQueryHandler : IRequestHandler<GetUploadUrlQuery, Signeds3uploadResponse>
    {
        private readonly IAutodeskPlatformService _autodeskPlatformService;
        private readonly ILogger _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public GetUploadUrlQueryHandler(ILogger<GetUploadUrlQueryHandler> logger, IAutodeskPlatformService autodeskPlatformService,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _autodeskPlatformService = autodeskPlatformService;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<Signeds3uploadResponse> Handle(GetUploadUrlQuery request, CancellationToken cancellationToken)
        {
            string bucketKey = _forgeConfiguration.InputBucketKey;
            string objectKey = request.ObjectKey + ".rvt";
            string requestIdPrefix = "";
            string? uploadKey = null;

            string requestId = HandleRequestId(requestIdPrefix, bucketKey, objectKey);

            Signeds3uploadResponse signedUrlResponse = await GetUploadUrlsWithRetry(
                bucketKey, objectKey, request.ChunksNumber, uploadKey, requestId);

            return signedUrlResponse;
        }

        private async Task<Signeds3uploadResponse> GetUploadUrlsWithRetry(string bucketKey, string objectKey, int numberOfChunks, string? uploadKey, string requestId)
        {
            int attemptCount = 0;

            do
            {
                _logger.LogInformation("{requestId} Refreshing URL attempt:{attemptCount}.", requestId, attemptCount);

                try
                {
                    Signeds3uploadResponse response = await _autodeskPlatformService.SignedS3UploadAsync(
                        bucketKey: bucketKey,
                        objectKey: objectKey,
                        parts: numberOfChunks,
                        uploadKey: uploadKey);

                    return response;
                }
                catch (OssApiException e)
                {
                    _logger.LogWarning("{requestId} Error: {errorMessage}", requestId, e.Message);
                    throw;
                }
            } while (attemptCount < 10);

            throw new OssApiException($"{requestId} Error: Fail getting upload urls after maximum retry");
        }

        private string HandleRequestId(string parentRequestId, string bucketKey, string objectKey)
        {
            string requestId = !string.IsNullOrEmpty(parentRequestId) ? parentRequestId : Guid.NewGuid().ToString();
            requestId = requestId + ":" + GenerateSdkRequestId(bucketKey, objectKey);
            return requestId;
        }

        private string GenerateSdkRequestId(string bucketKey, string objectKey)
        {
            return bucketKey + "/" + objectKey;
        }
    }
}
