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

namespace Application.Files.Queries.GetDownloadUrlQuery
{
    public class GetDownloadUrlQueryHandler : IRequestHandler<GetDownloadUrlQuery, Signeds3downloadResponse>
    {
        private readonly OssClient _ossClient;
        private readonly ILogger _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;

        private readonly string _accessTokenExpiredMessage = "Access token provided is invalid or expired.";
        private readonly string _forbiddenMessage = "403 (Forbidden)";

        public GetDownloadUrlQueryHandler(ILogger<GetDownloadUrlQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<Signeds3downloadResponse> Handle(GetDownloadUrlQuery request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataRead });

            string bucketKey = _forgeConfiguration.OutputBucketKey;
            string objectKey = request.ObjectKey;

            Signeds3downloadResponse signedUrl = await _ossClient.SignedS3DownloadAsync(
                twoLeggedToken.AccessToken, bucketKey, objectKey, minutesExpiration: 60);


            return signedUrl;
        }

    }


}
