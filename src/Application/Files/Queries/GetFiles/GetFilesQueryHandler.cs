using MediatR;
using Autodesk.Oss.Model;
using Autodesk.Oss;
using Microsoft.Extensions.Logging;
using Autodesk.Authentication.Model;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Autodesk.Authentication;

namespace Application.Files.Queries.GetFiles
{
    public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, BucketObjects>
    {
        private readonly OssClient _ossClient;
        private readonly ILogger _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        private readonly string _accessTokenExpiredMessage = "Access token provided is invalid or expired.";
        private readonly string _forbiddenMessage = "403 (Forbidden)";

        public GetFilesQueryHandler(ILogger<GetFilesQuery> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<BucketObjects> Handle(GetFilesQuery request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataRead });

            BucketObjects objects = await _ossClient.GetObjectsAsync(twoLeggedToken.AccessToken, request.BucketKey);

            return objects;
        }

    }


}
