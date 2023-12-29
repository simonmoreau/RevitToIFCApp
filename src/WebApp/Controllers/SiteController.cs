using Application.Objets.Commands.CreateObjetCommand;
using Application.Sites.Queries.GetSiteList;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controls for the sites
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SiteController : BaseController
    {
        [HttpGet(Name = "GetSites")]
        public async Task<IEnumerable<Domain.Entities.Site>> GetAllSites()
        {
            var vm = await Mediator.Send(new GetSiteListQuery());
            return vm.Sites;
        }
    }
}
