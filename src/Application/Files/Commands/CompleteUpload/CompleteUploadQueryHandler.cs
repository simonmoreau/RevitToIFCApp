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
using Azure;
using System.Text.Json;

namespace Application.Files.Commands.CompleteUpload
{
    public class CompleteUploadQueryHandler : IRequestHandler<CompleteUploadQuery, CompleteUploadResponse>
    {
        private readonly OssClient _ossClient;
        private readonly ILogger _logger;
        private readonly AuthenticationClient _authenticationClient;
        private readonly ForgeConfiguration _forgeConfiguration;


        public CompleteUploadQueryHandler(ILogger<CompleteUploadQueryHandler> logger, AuthenticationClient authenticationClient,
            IOptions<ForgeConfiguration> forgeConfiguration, OssClient ossClient)
        {
            _ossClient = ossClient;
            _logger = logger;
            _authenticationClient = authenticationClient;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<CompleteUploadResponse> Handle(CompleteUploadQuery request, CancellationToken cancellationToken)
        {
            TwoLeggedToken twoLeggedToken = await _authenticationClient.GetTwoLeggedTokenAsync(_forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite });

            string bucketKey = _forgeConfiguration.InputBucketKey;
            string objectKey = request.ObjectKey + ".rvt";

            Completes3uploadBody completes3UploadBody = new Completes3uploadBody();
            completes3UploadBody.UploadKey = request.UploadKey;
            completes3UploadBody.Size = null; // (int)request.Size;
            completes3UploadBody.ETags = request.ETags;

            HttpResponseMessage response = await _ossClient.CompleteSignedS3UploadAsync(
                twoLeggedToken.AccessToken, bucketKey, objectKey, "application/json", completes3UploadBody);

            CompleteUploadResponse? completeUploadResponse = await JsonSerializer.DeserializeAsync<CompleteUploadResponse>(await response.Content.ReadAsStreamAsync());
            return completeUploadResponse;
        }
    }
}
