using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Services.Intarfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.Exceptions;
using WebApi.Token;
using WebApi.ViewModel;

namespace WebApi.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class LoginController : BaseController
    {
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private ILogger<LoginController> _logger;

        public LoginController(IUserService userService, SignInManager<User> signInManager, UserManager<User> userManager, ILogger<LoginController> logger)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Token([FromBody] LoginModel model)
        {
            ClaimsIdentity identity = null;

            try
            {
                identity = await GetIdentity(model.Username, model.Password);
            }
            catch(Exception ex)
            {
                throw new AuthorizeException("Unauthorize", 401); 
            }
            
            if(identity == null)
                throw new AuthorizeException("Unauthorize", 401); ;

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
           
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            User user = await _userService.GetUser(x => x.UserName == identity.Name);

            var response = new
            {
                access_token = encodedJwt,
                userId = user.Id,
                role = user.Role,
                firstname = user.FirstName,
                lastname = user.LastName
            };

            return Ok(response);
        }


        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            User user = await _userService.GetUser(x => x.UserName == username);
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
         
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, (await _userManager.GetRolesAsync(user)).FirstOrDefault()),
                };

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
