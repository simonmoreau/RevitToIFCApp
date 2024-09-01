using Application.Files.Commands.CompleteUpload;
using Application.Files.Commands.CreateBucket;
using Application.Files.Queries.GetDownloadUrlQuery;
using Application.Files.Queries.GetFiles;
using Application.Files.Queries.GetUploadUrl;
using Application.WorkItems.Commands.CreateWorkItem;
using Application.WorkItems.Queries.GetWorkItemStatus;
using Autodesk.Forge.DesignAutomation.Model;
using Autodesk.Oss.Model;
using Domain.Entities;
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
        public async Task<Signeds3uploadResponse> GetUploadUrls(int chunksNumber, string objectKey)
        {
            Signeds3uploadResponse vm = await Mediator.Send(new GetUploadUrlQuery(chunksNumber, objectKey));
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
            CompleteUploadResponse vm = await Mediator.Send(new CompleteUploadQuery(
                uploadCompletion.uploadKey, uploadCompletion.size, uploadCompletion.eTags, uploadCompletion.objectKey));
            return vm;
        }

        /// <summary>
        /// Get download url
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("download")]
        public async Task<Signeds3downloadResponse> GetDownloadUrls(string objectKey, string fileName)
        {
            Signeds3downloadResponse vm = await Mediator.Send(new GetDownloadUrlQuery(objectKey, fileName));
            return vm;
        }

        /// <summary>
        /// List objects in bucket
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("objects")]
        public async Task<BucketObjects> GetObjects(string bucketKey)
        {
            BucketObjects bucketObjects = await Mediator.Send(new GetFilesQuery(bucketKey));
            return bucketObjects;
        }

        /// <summary>
        /// Create a work item
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("workItem")]
        public async Task<WorkItemStatus> CreateWorkItem(WorkItemCreation workItemCreation)
        {
            WorkItemStatus vm = await Mediator.Send(new CreateWorkItemCommand(workItemCreation));
            return vm;
        }

        /// <summary>
        /// Get a workITemStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("status")]
        public async Task<WorkItemStatus> CompleteUpload(string id)
        {
            WorkItemStatus vm = await Mediator.Send(new GetWorkItemStatusQuery(id));

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
