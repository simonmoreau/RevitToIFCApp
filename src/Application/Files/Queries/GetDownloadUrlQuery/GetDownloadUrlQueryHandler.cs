using Application.Common.Services;
using Autodesk.Oss.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Files.Queries.GetDownloadUrlQuery
{
    public class GetDownloadUrlQueryHandler : IRequestHandler<GetDownloadUrlQuery, Signeds3downloadResponse>
    {
        private readonly IAutodeskPlatformService _autodeskPlatformService;
        private readonly ILogger _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public GetDownloadUrlQueryHandler(ILogger<GetDownloadUrlQueryHandler> logger, IAutodeskPlatformService autodeskPlatformService,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _autodeskPlatformService = autodeskPlatformService;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<Signeds3downloadResponse> Handle(GetDownloadUrlQuery request, CancellationToken cancellationToken)
        {
            string bucketKey = _forgeConfiguration.OutputBucketKey;
            string objectKey = request.ObjectKey + ".ifc";
            string outputFileName = Path.GetFileNameWithoutExtension(request.FileName) + ".ifc";

            Signeds3downloadResponse signedUrl = await _autodeskPlatformService.SignedS3DownloadAsync(
                bucketKey, objectKey,
                responseContentDisposition: $"attachment; filename=\"{outputFileName}\"");

            if (signedUrl == null)
            {
                signedUrl = new Signeds3downloadResponse();
            }

            return signedUrl;
        }
    }
}
