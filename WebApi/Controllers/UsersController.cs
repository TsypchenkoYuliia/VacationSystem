﻿using BusinessLogic.Services.Intarfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAll([FromQuery] string name = null, [FromQuery] string role = null)
        {
            var users = await _userService.GetUsers(name, role);

            return users;
        }

        [HttpGet("statistics/{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            var user = await _userService.GetUser(userId);
            return Ok(user);
        }
    }
}
