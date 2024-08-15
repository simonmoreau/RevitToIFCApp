using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

namespace Application.ForgeApplications.Commands.CreateForgeApplication
{

    public class CreateForgeApplicationCommand : IRequest<AppBundle>
    {
        public string? RevitVersion { get; set; }
        public string? AppbundleFile { get; set; }

    }
}
