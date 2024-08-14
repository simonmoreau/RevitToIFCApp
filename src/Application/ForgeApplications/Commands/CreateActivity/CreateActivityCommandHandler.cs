using Autodesk.Forge.DesignAutomation;
using Autodesk.Forge.DesignAutomation.Model;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.ForgeApplications.Commands.CreateActivity
{
    public class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, Activity>
    {
        private readonly DesignAutomationClient _designAutomationClient;
        private readonly Domain.Entities.ForgeConfiguration _forgeConfiguration;

        public CreateActivityCommandHandler(DesignAutomationClient designAutomationClient, IOptions<ForgeConfiguration> forgeConfiguration)
        {
            _designAutomationClient = designAutomationClient;
            _forgeConfiguration = forgeConfiguration.Value;
        }

        public async Task<Activity> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
        {
            // standard name for this sample
            string appBundleName = $"{_forgeConfiguration.ApplicationDetail.AppBundleName}AppBundle";
            string activityName = $"{_forgeConfiguration.ApplicationDetail.AppBundleName}Activity";

            string nickname = _forgeConfiguration.ApplicationDetail.Nickname;
            string alias = _forgeConfiguration.ApplicationDetail.Alias;

            string description = _forgeConfiguration.ApplicationDetail.Description;
            string engineName = request.Engine;

            string outputFile = _forgeConfiguration.ApplicationDetail.OutputFileName;


            // define the activity
            // ToDo: parametrize for different engines...
            EngineAttribute engineAttributes = EngineAttribute.Revit;
            string commandLine = string.Format(engineAttributes.CommandLine, appBundleName);

            Activity activitySpec = new Activity()
            {
                Id = activityName,
                Appbundles = new List<string>() { string.Format("{0}.{1}+{2}", nickname, appBundleName, alias) },
                CommandLine = new List<string>() { commandLine },
                Engine = engineName,
                Parameters = new Dictionary<string, Parameter>()
                    {
                        { "inputFile", new Parameter()
                            { Description = "input Revit file",
                                LocalName = "$(inputFile)",
                                Ondemand = false,
                                Required = true,
                                Verb = Verb.Get,
                                Zip = false
                            }
                        },
                        { "inputJson", new Parameter()
                            { Description = "input json",
                                LocalName = "params.json",
                                Ondemand = false,
                                Required = false,
                                Verb = Verb.Get,
                                Zip = false
                            }
                        },
                        { "outputFile", new Parameter()
                            { Description = "output IFC model file",
                                LocalName = outputFile + ".ifc",
                                Ondemand = false,
                                Required = false,
                                Verb = Verb.Put,
                                Zip = false
                            }
                        }
                    },
                Settings = new Dictionary<string, ISetting>()
                    {
                        { "script", new StringSetting(){ Value = engineAttributes.Script } }
                    }
            };

            Page<string> activities = await _designAutomationClient.GetActivitiesAsync();

            string qualifiedActivityId = string.Format("{0}.{1}+{2}", nickname, activityName, alias);

            if (!activities.Data.Contains(qualifiedActivityId))
            {
                Activity newActivity = await _designAutomationClient.CreateActivityAsync(activitySpec);

                // specify the alias for this Activity
                Alias aliasSpec = new Alias() { Id = alias, Version = 1 };
                Alias newAlias = await _designAutomationClient.CreateActivityAliasAsync(activityName, aliasSpec);

                return newActivity;
            }
            else
            {
                int version = await _designAutomationClient.UpdateActivityAsync(activitySpec, alias);
                Activity newActivity = await _designAutomationClient.GetActivityAsync(qualifiedActivityId);
                return newActivity;
            }

        }
    }

    public class EngineAttribute
    {
        public static EngineAttribute DsMax
        {
            get { return new EngineAttribute("$(engine.path)\\3dsmaxbatch.exe -sceneFile \"$(args[inputFile].path)\" \"$(settings[script].path)\"", "max", "da = dotNetClass(\"Autodesk.Forge.Sample.DesignAutomation.Max.RuntimeExecute\")\nda.ModifyWindowWidthHeight()\n"); }
        }

        public static EngineAttribute AutoCAD
        {
            get { return new EngineAttribute("$(engine.path)\\accoreconsole.exe /i \"$(args[inputFile].path)\" /al \"$(appbundles[{0}].path)\" /s \"$(settings[script].path)\"", "dwg", "UpdateParam\n"); }
        }

        public static EngineAttribute Inventor
        {
            get { return new EngineAttribute("$(engine.path)\\InventorCoreConsole.exe /i \"$(args[inputFile].path)\" /al \"$(appbundles[{0}].path)\"", "ipt", string.Empty); }
        }


        public static EngineAttribute Revit
        {
            get { return new EngineAttribute("$(engine.path)\\revitcoreconsole.exe /i \"$(args[inputFile].path)\" /al \"$(appbundles[{0}].path)\"", "rvt", string.Empty); }
        }

        private EngineAttribute(string commandLine, string extension, string script)
        {
            CommandLine = commandLine;
            Extension = extension;
            Script = script;
        }

        public string CommandLine { get; private set; }
        public string Extension { get; private set; }
        public string Script { get; private set; }

    }
}
