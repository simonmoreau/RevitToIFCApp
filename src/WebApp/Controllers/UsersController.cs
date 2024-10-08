﻿using Application.Activities.Queries.ListActivities;
using Application.Users.Queries.GetAllUsers;
using Application.Users.Queries.GetUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace WebApp.Controllers
{
    /// <summary>
    /// Manage users
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Get me
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetMe")]
        public async Task<UserDTO> GetMe()
        {
            string? id = User.GetObjectId();
            UserDTO user = await Mediator.Send(new GetUserQuery(id));
            return user;
        }
    }
}
