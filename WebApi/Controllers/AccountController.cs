using BusinessLogic.Services.Intarfaces;
using DataAccess.Context.Enum;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(Roles = "Admin")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public AccountController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {                
                if (Enum.IsDefined(typeof(RoleName), model.Role))
                {
                        User newUser = await _userService.CreateUser(new User()
                        {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.Email,
                        Role = model.Role,
                        }, model.Password);

                    await _userManager.AddToRoleAsync(newUser, model.Role);

                    return Ok(newUser);
                }
                else
                    throw new CreateException("Role is not valid", 422);
            }
            else
                throw new CreateException("User not created: Invalid user data", 422);
        }

        [HttpGet]
        public async Task<IEnumerable<User>> GetAll([FromQuery] string name = null, [FromQuery] string role = null)
        {
            var users = await _userService.GetUsers(name, role);

            return users;
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.DeleteUser(userId);

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            await _userService.UpdateUser(user);

            return Ok();
        }
    }
}
