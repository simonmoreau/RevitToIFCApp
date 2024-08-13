using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ForgeApplications.Commands.CreateActivity
{
    public class CreateActivityCommand : IRequest<Activity>
    {
        public string? Engine { get; set; }
    }
}
