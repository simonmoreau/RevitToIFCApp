using Application.Common.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ForgeApplications.Commands.CreateForgeApplication
{

    public class CreateForgeApplicationCommand : IRequest<string>
    {
        public string? Engine { get; set; }
        public string? Name { get; set; }
        public string? AppbundleFile { get; set; }
        public string? Description { get; set; }

    }
}
