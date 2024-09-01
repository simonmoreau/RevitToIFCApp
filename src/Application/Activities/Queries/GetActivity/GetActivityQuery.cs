using MediatR;
using Autodesk.Forge.DesignAutomation.Model;

namespace Application.Activities.Queries.GetActivity
{
    public class GetActivityQuery : IRequest<Activity>
    {
        public string Id { get; set; }
    }
}
