using Application.Activities.Queries.GetActivity;
using Application.Activities.Queries.ListActivities;
using Application.Files.Queries;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Application.Sites.Queries.GetSiteList;
using Autodesk.Forge.DesignAutomation.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;


namespace WebApp.Controllers
{
    /// <summary>
    /// Manage files
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FilesController : BaseController
    {
        /// <summary>
        /// Get upload urls
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetUploadUrls")]
        public async Task<IEnumerable<string>> GetUploadUrls()
        {
            GetUploadUrlVm vm = await Mediator.Send(new GetUploadUrlQuery());
            return vm.Urls;
        }


    }
}
