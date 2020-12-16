using BusinessLogic.Services.Intarfaces;
using DataAccess.Context.Enum;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Exceptions;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = RoleName.admin)]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User newUser = await _userService.CreateUser(new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                }, model.Password);

                return Ok(newUser);
            }
            else
                throw new CreateException("User not created: Invalid user data", 422);
        }
    }
}
