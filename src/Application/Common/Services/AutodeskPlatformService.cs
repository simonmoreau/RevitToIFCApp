using Autodesk.Authentication;
using Autodesk.Authentication.Model;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss;
using Autodesk.Oss.Http;
using Autodesk.Oss.Model;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Common.Services
{
    internal sealed class AutodeskPlatformService : IAutodeskPlatformService
    {
        private readonly AuthenticationClient _authenticationClient;
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly OssClient _ossClient;
        private readonly IBucketsApi _bucketsApi;
        private readonly ForgeConfiguration _forgeConfiguration;

        public AutodeskPlatformService(
            AuthenticationClient authenticationClient,
            DesignAutomationClient designAutomationClient,
            OssClient ossClient,
            IBucketsApi bucketsApi,
            IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _authenticationClient = authenticationClient;
            _designAutomationClient = designAutomationClient;
            _ossClient = ossClient;
            _bucketsApi = bucketsApi;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<string> GetAccessTokenAsync(List<Scopes> scopes)
        {
            TwoLeggedToken token = await _authenticationClient.GetTwoLeggedTokenAsync(
                _forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, scopes);

            return token.AccessToken;
        }

        public async Task<Signeds3uploadResponse> SignedS3UploadAsync(string bucketKey, string objectKey, int parts, string? uploadKey)
        {
            TwoLeggedToken token = await _authenticationClient.GetTwoLeggedTokenAsync(
                _forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite });

            Signeds3uploadResponse response = await _ossClient.SignedS3UploadAsync(
                bucketKey: bucketKey,
                objectKey: objectKey,
                parts: parts,
                uploadKey: uploadKey,
                accessToken: token.AccessToken,
                minutesExpiration: 15,
                xAdsAcmScopes: "data:write");

            return response;
        }

        public async Task<BucketObjects> GetObjectsAsync(string bucketKey)
        {
            TwoLeggedToken token = await _authenticationClient.GetTwoLeggedTokenAsync(
                _forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataRead });

            BucketObjects objects = await _ossClient.GetObjectsAsync(token.AccessToken, bucketKey);

            return objects;
        }

        public async Task<Signeds3downloadResponse> SignedS3DownloadAsync(string bucketKey, string objectKey, string responseContentDisposition)
        {
            TwoLeggedToken token = await _authenticationClient.GetTwoLeggedTokenAsync(
                _forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataRead });

            Signeds3downloadResponse signedUrl = await _ossClient.SignedS3DownloadAsync(
                token.AccessToken, bucketKey, objectKey,
                responseContentDisposition: responseContentDisposition,
                minutesExpiration: 60,
                throwOnError: false);

            return signedUrl;
        }

        public async Task<HttpResponseMessage> CompleteSignedS3UploadAsync(string bucketKey, string objectKey, Completes3uploadBody completes3UploadBody)
        {
            TwoLeggedToken token = await _authenticationClient.GetTwoLeggedTokenAsync(
                _forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.DataWrite });

            HttpResponseMessage response = await _ossClient.CompleteSignedS3UploadAsync(
                token.AccessToken, bucketKey, objectKey, "application/json", completes3UploadBody);

            return response;
        }

        public async Task<Bucket> CreateBucketAsync(CreateBucketsPayload payload, Region region)
        {
            TwoLeggedToken token = await _authenticationClient.GetTwoLeggedTokenAsync(
                _forgeConfiguration.ClientId, _forgeConfiguration.ClientSecret, new List<Scopes> { Scopes.BucketCreate });

            Autodesk.Forge.Core.ApiResponse<Bucket> response = await _bucketsApi.CreateBucketAsync(payload, region, token.AccessToken);

            return response.Content;
        }

        public async Task<WorkItemStatus> CreateWorkItemAsync(WorkItem workItem)
        {
            WorkItemStatus workItemStatus = await _designAutomationClient.CreateWorkItemAsync(workItem);

            return workItemStatus;
        }
    }
}
