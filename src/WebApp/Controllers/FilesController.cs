using Application.Activities.Queries.GetActivity;
using Application.Activities.Queries.ListActivities;
using Application.Files.Commands.CreateBucket;
using Application.Files.Queries.GetUploadUrl;
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
        public async Task<IEnumerable<string>> GetUploadUrls(int chunksNumber)
        {
            GetUploadUrlVm vm = await Mediator.Send(new GetUploadUrlQuery(chunksNumber));
            return vm.Urls;
        }

        /// <summary>
        /// Create a new bucket
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateBucket()
        {
            try
            {
                string bucketKey = await Mediator.Send(new CreateBucketCommand());

                if (bucketKey == null)
                    return BadRequest();

                return CreatedAtAction(nameof(CreateBucketCommand),
                    new { id = bucketKey });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating the application");
            }

        }


    }
}
