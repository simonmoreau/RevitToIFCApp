using Application.Sites.Queries.GetSiteList;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.DesignAutomation.Model;

namespace Application.Activities.Queries.GetActivity
{
    public class GetActivityQuery : IRequest<Activity>
    {
        public string Id { get; set; }
    }
}
