using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Autodesk.Authentication;
using Autodesk.Oss.Http;
using Autodesk.Oss.Model;
using Autodesk.Authentication.Model;

namespace Application.Files.Commands.CreateBucket
{
    public class CreateBucketCommandHandler : IRequestHandler<CreateBucketCommand, string>
    {
        private readonly IBucketsApi _bucketsApi;
        private readonly ILogger<CreateBucketCommandHandler> _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly IConfiguration _configuration;
        private readonly ForgeConfiguration _forgeConfiguration;

        public CreateBucketCommandHandler(ILogger<CreateBucketCommandHandler> logger, AuthenticationClient authenticationClient,
    IConfiguration configuration, IOptions<ForgeConfiguration> forgeConfiguration, IBucketsApi bucketsApi)
        {
            _bucketsApi = bucketsApi;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _configuration = configuration;
            _forgeConfiguration = forgeConfiguration.Value;

        }

        public async Task<string> Handle(CreateBucketCommand request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.BucketCreate });

            var payload = new CreateBucketsPayload();
            payload.BucketKey = _forgeConfiguration.BucketKey;

            Autodesk.Forge.Core.ApiResponse<Bucket> response = await _bucketsApi.CreateBucketAsync(payload,Region.EMEA, twoLeggedToken.AccessToken);

            Bucket bucket = response.Content;

             return bucket.BucketKey;
        }
    }
}
