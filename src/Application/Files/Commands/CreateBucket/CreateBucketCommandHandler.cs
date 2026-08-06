using Application.Common.Services;
using Autodesk.Oss.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Files.Commands.CreateBucket
{
    public class CreateBucketCommandHandler : IRequestHandler<CreateBucketCommand, string>
    {
        private readonly IAutodeskPlatformService _autodeskPlatformService;
        private readonly ILogger<CreateBucketCommandHandler> _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CreateBucketCommandHandler(ILogger<CreateBucketCommandHandler> logger, IAutodeskPlatformService autodeskPlatformService,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _autodeskPlatformService = autodeskPlatformService;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<string> Handle(CreateBucketCommand request, CancellationToken cancellationToken)
        {
            await CreateBucket(_forgeConfiguration.InputBucketKey);
            await CreateBucket(_forgeConfiguration.OutputBucketKey);

            return _forgeConfiguration.OutputBucketKey;
        }

        private async Task<string> CreateBucket(string bucketKey)
        {
            CreateBucketsPayload payload = new CreateBucketsPayload();
            payload.BucketKey = bucketKey;
            payload.PolicyKey = PolicyKey.Temporary;

            Bucket bucket = await _autodeskPlatformService.CreateBucketAsync(payload, Region.EMEA);

            return bucket.BucketKey;
        }
    }
}
