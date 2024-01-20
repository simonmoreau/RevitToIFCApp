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
            string appBundleName = $"{request.Name}App";
            // Create or update an appbundle
            AppBundle appBundleBody = new AppBundle();
            appBundleBody.Engine = request.Engine;
            appBundleBody.Description = request.Description;
            appBundleBody.Id = appBundleName;

            Autodesk.Forge.Core.ApiResponse<AppBundle> createdAppBundleResponse = await _appBundlesApi.CreateAppBundleAsync(appBundleBody);
            AppBundle createdAppBundle = createdAppBundleResponse.Content;

            // Create or update an appbundle alias
            Alias aliasBody = new Alias();
            aliasBody.Version = 1;
            aliasBody.Id = $"Alias_{request.Name}";

            Autodesk.Forge.Core.ApiResponse<Alias> createdAliasResponse = await _appBundlesApi.CreateAppBundleAliasAsync(appBundleName, aliasBody);
            Alias createdAlias = createdAliasResponse.Content;

            // Upload to the appBundle
            

            // Create or update an activity
            Activity activity = new Activity();
            activity.Id = $"Activity{request.Name}";
            activity.Description = "Revit 2024 Ifc Export Activity";

            string engineCommand = "$(engine.path)\\revitcoreconsole.exe";
            string inputCommand = "$(args[rvtFile].path)";
            string appBundleCommand = $"$(appbundles[{appBundleName}].path)";
            activity.CommandLine = new List<string> { $"\"{engineCommand}\" /i \"{inputCommand}\" /al \"{appBundleCommand}\"" };

            activity.Parameters = new Dictionary<string, Parameter>();
            Parameter inputParameter = new Parameter();
            inputParameter.Zip = false;
            inputParameter.Ondemand = false;
            inputParameter.Verb = Verb.Get;
            inputParameter.Description = "Input Revit model";
            inputParameter.Required = true;
            inputParameter.LocalName = "$(rvtFile)";

            activity.Parameters.Add("rvtFile", inputParameter);

            Parameter resultParameter = new Parameter();
            resultParameter.Zip = false;
            resultParameter.Ondemand = false;
            resultParameter.Verb = Verb.Put;
            resultParameter.Description = "Resulting exported model";
            resultParameter.Required = true;
            resultParameter.LocalName = "result.ifc";

            activity.Parameters.Add("result", resultParameter);

            activity.Engine = request.Engine;

            activity.Appbundles = new List<string> { $"{createdAppBundle.Id}+test" };

            Autodesk.Forge.Core.ApiResponse<Activity> createdActivityResponse = await _activitiesApi.CreateActivityAsync(activity);
            Activity createdActivity = createdActivityResponse.Content;

            // Create or update an activity alias

            aliasBody = new Alias();
            aliasBody.Version = 1;
            aliasBody.Id = $"Alias{request.Name}";

            Autodesk.Forge.Core.ApiResponse<Alias> createdActiviyAliasResponse = await _activitiesApi.CreateActivityAliasAsync(activity.Id, aliasBody);
            Alias createdActivityAlias = createdActiviyAliasResponse.Content;


            return createdAppBundle.Id;

        }
    }
}
