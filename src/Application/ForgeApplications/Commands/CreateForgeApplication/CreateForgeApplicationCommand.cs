using Application.Common.Interfaces;
using Application.Services;
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

    public class CreateForgeApplicationCommand : IRequest<AppBundle>
    {
        public string? Engine { get; set; }
        public string? AppbundleFile { get; set; }

    }
}
