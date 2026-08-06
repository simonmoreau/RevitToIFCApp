using Application.Common.Services;
using Autodesk.Oss.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Application.Files.Commands.CompleteUpload
{
    public class CompleteUploadQueryHandler : IRequestHandler<CompleteUploadQuery, CompleteUploadResponse>
    {
        private readonly IAutodeskPlatformService _autodeskPlatformService;
        private readonly ILogger _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CompleteUploadQueryHandler(ILogger<CompleteUploadQueryHandler> logger, IAutodeskPlatformService autodeskPlatformService,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _autodeskPlatformService = autodeskPlatformService;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<CompleteUploadResponse> Handle(CompleteUploadQuery request, CancellationToken cancellationToken)
        {
            string bucketKey = _forgeConfiguration.InputBucketKey;
            string objectKey = request.ObjectKey + ".rvt";

            Completes3uploadBody completes3UploadBody = new Completes3uploadBody();
            completes3UploadBody.UploadKey = request.UploadKey;
            completes3UploadBody.Size = null;
            completes3UploadBody.ETags = request.ETags;

            HttpResponseMessage response = await _autodeskPlatformService.CompleteSignedS3UploadAsync(
                bucketKey, objectKey, completes3UploadBody);

            CompleteUploadResponse? completeUploadResponse = await JsonSerializer.DeserializeAsync<CompleteUploadResponse>(await response.Content.ReadAsStreamAsync());
            return completeUploadResponse;
        }
    }
}
