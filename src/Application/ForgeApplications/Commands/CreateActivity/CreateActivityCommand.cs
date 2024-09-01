using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

namespace Application.ForgeApplications.Commands.CreateActivity
{
    public class CreateActivityCommand : IRequest<Activity>
    {
        public string? RevitVersion { get; set; }
    }
}
