using Autodesk.Forge.DesignAutomation.Model;
using MediatR;

namespace Application.ForgeApplications.Commands.CreateActivity
{
    public class CreateActivityCommand : IRequest<Activity>
    {
        public string? Engine { get; set; }
    }
}
