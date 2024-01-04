using Application.Common.Interfaces;
using Application.Objets.Commands.CreateObjetCommand;
using Application.Services;
using Autodesk.Forge.DesignAutomation.Http;
using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ForgeApplications.Commands.CreateForgeApplication
{
    public class CreateForgeApplicationCommandHandler : IRequestHandler<CreateForgeApplicationCommand, string>
    {
        private readonly IActivitiesApi _activitiesApi;
        private readonly IAppBundlesApi _appBundlesApi;

        public CreateForgeApplicationCommandHandler(IActivitiesApi activitiesApi, IAppBundlesApi appBundlesApi)
        {
            _activitiesApi = activitiesApi;
            _appBundlesApi = appBundlesApi;
        }

        public async Task<string> Handle(CreateForgeApplicationCommand request, CancellationToken cancellationToken)
        {
            // Create or update an appbundle
            AppBundle appBundleBody = new AppBundle();
            appBundleBody.Engine = request.Engine;
            appBundleBody.Description = request.Description;
            appBundleBody.Appbundles.Add(request.AppbundleFile);

            Autodesk.Forge.Core.ApiResponse<AppBundle> createdAppBundleResponse = await _appBundlesApi.CreateAppBundleAsync(appBundleBody);
            AppBundle createdAppBundle = createdAppBundleResponse.Content;

            // Create or update an appbundle alias
            Alias aliasBody = new Alias();
            aliasBody.Version = 1;
            aliasBody.Id = "myAlias";

            Autodesk.Forge.Core.ApiResponse<Alias> createdAliasResponse = await _appBundlesApi.CreateAppBundleAliasAsync(createdAppBundle.Id, aliasBody);
            Alias createdAlias = createdAliasResponse.Content;


            // Create or update an activity
            // Create or update an activity alias



            return createdAppBundle.Id;

        }
    }
}
