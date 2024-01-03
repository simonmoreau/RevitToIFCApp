using Application.Sites.Queries.GetSiteList;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.WorkItems.Queries.GetWorkItem
{
    public class GetWorkItemQuery : IRequest<WorkItem>
    {
    }
}
