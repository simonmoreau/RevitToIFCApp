using Application.Activities.Queries.GetActivity;
using Application.Activities.Queries.ListActivities;
using Application.Files.Commands.CompleteUpload;
using Application.Files.Commands.CreateBucket;
using Application.Files.Queries.GetUploadUrl;
using Application.ForgeApplications.Commands.CreateForgeApplication;
using Application.Sites.Queries.GetSiteList;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss.Model;
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
        public async Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber)
        {
            Signeds3uploadResponse vm = await Mediator.Send(new GetUploadUrlQuery(chunksNumber));
            return vm;
        }

        /// <summary>
        /// Complete the upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("complete")]
        public async Task<CompleteUploadResponse> CompleteUpload(UploadCompletion uploadCompletion)
        {
            CompleteUploadResponse vm = await Mediator.Send(new CompleteUploadQuery(uploadCompletion.uploadKey, uploadCompletion.size, uploadCompletion.eTags));
            return vm;
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
