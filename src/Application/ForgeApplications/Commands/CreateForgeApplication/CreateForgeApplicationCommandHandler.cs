using Application.Common.Interfaces;
using Application.Objets.Commands.CreateObjetCommand;
using Application.Services;
using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ForgeApplications.Commands.CreateForgeApplication
{
    public class CreateForgeApplicationCommandHandler : IRequestHandler<CreateForgeApplicationCommand, AppBundle>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly Domain.Entities.ForgeConfiguration _forgeConfiguration;

        public CreateForgeApplicationCommandHandler(DesignAutomationClient designAutomationClient, IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _designAutomationClient = designAutomationClient;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<AppBundle> Handle(CreateForgeApplicationCommand request, CancellationToken cancellationToken)
        {
            string appBundleName = $"{_forgeConfiguration.ApplicationDetail.AppBundleName}AppBundle";
            string nickname = _forgeConfiguration.ApplicationDetail.Nickname;
            string alias = _forgeConfiguration.ApplicationDetail.Alias;
            string description = _forgeConfiguration.ApplicationDetail.Description;
            string engineName = request.Engine;

            // check if ZIP with bundle is here
            string packageZipPath = request.AppbundleFile;
            if (!System.IO.File.Exists(packageZipPath))
            {
                throw new Exception("Appbundle not found at " + packageZipPath);
            }

            // get defined app bundles
            Page<string> appBundles = await _designAutomationClient.GetAppBundlesAsync();

            // check if app bundle is already define
            AppBundle newAppVersion;
            string qualifiedAppBundleId = string.Format("{0}.{1}+{2}", nickname, appBundleName, alias);
            if (!appBundles.Data.Contains(qualifiedAppBundleId))
            {
                // create an appbundle (version 1)
                AppBundle appBundleSpec = new AppBundle()
                {
                    Package = appBundleName,
                    Engine = engineName,
                    Id = appBundleName,
                    Description = description,

                };

                newAppVersion = await _designAutomationClient.CreateAppBundleAsync(appBundleSpec);
                if (newAppVersion == null)
                {
                    throw new Exception("Cannot create new app");
                }

                // create alias pointing to v1
                Alias aliasSpec = new Alias() { Id = alias, Version = 1 };
                Alias newAlias = await _designAutomationClient.CreateAppBundleAliasAsync(appBundleName, aliasSpec);
            }
            else
            {
                // create new version
                AppBundle appBundleSpec = new AppBundle()
                {
                    Engine = engineName,
                    Description = appBundleName
                };

                newAppVersion = await _designAutomationClient.CreateAppBundleVersionAsync(appBundleName, appBundleSpec);
                if (newAppVersion == null)
                {
                    throw new Exception("Cannot create new version");
                }

                // update alias pointing to v+1
                AliasPatch aliasSpec = new AliasPatch()
                {
                    Version = newAppVersion.Version.HasValue ? newAppVersion.Version.Value : 0,
                };
                Alias newAlias = await _designAutomationClient.ModifyAppBundleAliasAsync(appBundleName, alias, aliasSpec);
            }

            // upload the zip with .bundle
            await UploadAppBundleBits(newAppVersion.UploadParameters, packageZipPath);

            return newAppVersion;
        }

        public async Task UploadAppBundleBits(UploadAppBundleParameters uploadParameters, string packagePath)
        {
            using (MultipartFormDataContent formData = new MultipartFormDataContent())
            {
                foreach (KeyValuePair<string, string> kv in uploadParameters.FormData)
                {
                    if (kv.Value != null)
                    {
                        formData.Add(new StringContent(kv.Value), kv.Key);
                    }
                }

                using (StreamContent content = new StreamContent(new FileStream(packagePath, FileMode.Open)))
                {
                    formData.Add(content, "file");

                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uploadParameters.EndpointURL) { Content = formData })
                    {
                        request.Options.Set(Autodesk.Forge.Core.ForgeConfiguration.TimeoutKey, (int)TimeSpan.FromMinutes(10).TotalSeconds);

                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.SendAsync(request);
                            response.EnsureSuccessStatusCode();
                        }
                    }
                }
            }
        }
    }
}
