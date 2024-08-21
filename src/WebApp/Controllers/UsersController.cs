using Application.Activities.Queries.ListActivities;
using Application.Users.Queries.GetAllUsers;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    /// <summary>
    /// Manage users
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Get me
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetMe")]
        public async Task<string> GetMe()
        {
            string vm = await Mediator.Send(new GetAllUsersQuery());
            return vm;
        }
    }
}
