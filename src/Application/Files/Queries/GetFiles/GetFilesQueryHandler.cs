using Application.Common.Services;
using Autodesk.Oss.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Files.Queries.GetFiles
{
    public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, BucketObjects>
    {
        private readonly IAutodeskPlatformService _autodeskPlatformService;
        private readonly ILogger _logger;
        private readonly ForgeConfiguration _forgeConfiguration;

        public GetFilesQueryHandler(ILogger<GetFilesQuery> logger, IAutodeskPlatformService autodeskPlatformService,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _autodeskPlatformService = autodeskPlatformService;
            _logger = logger;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<BucketObjects> Handle(GetFilesQuery request, CancellationToken cancellationToken)
        {
            BucketObjects objects = await _autodeskPlatformService.GetObjectsAsync(request.BucketKey);

            return objects;
        }
    }
}
