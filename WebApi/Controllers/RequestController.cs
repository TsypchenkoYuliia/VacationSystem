using BusinessLogic.Services.Intarfaces;
using DataAccess.Context.Enum;
using Domain.DomainModel;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : BaseController
    {
        private IRequestService _service;
        private UserManager<User> _userManager;
        private readonly IUserService _userService;
        private ILogger<RequestController> _logger;

        public RequestController(IRequestService service, UserManager<User> userManager, IUserService userService, ILogger<RequestController> logger)
        {
            _service = service;
            _userManager = userManager;
            _userService = userService;
            _logger = logger;
        }


        [Authorize(Roles = "Manager, Accountant, Admin")]
        [HttpGet("/requests")]
        public async Task<IReadOnlyCollection<Request>> Get(string userId, DateTime? startDate = null, DateTime? endDate = null, int? stateId = null, int? typeId = null)
        {
            return await _service.GetAllAsync(userId, startDate, endDate, stateId, typeId);
        }

        [HttpGet("/user/requests")]
        public async Task<IReadOnlyCollection<Request>> Get(DateTime? startDate = null, DateTime? endDate = null, int? stateId = null, int? typeId = null)
        {
            User user = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            return await _service.GetAllAsync(user.Id, startDate, endDate, stateId, typeId);
        }

        [Authorize(Roles = "Manager, Accountant, Admin")]
        [HttpGet("/request/{requestId}")]
        public async Task<Request> Get(int requestId)
        {
            return await _service.GetByIdAsync(requestId);
        }

        [HttpGet("/user/request/{requestId}")]
        public async Task<Request> Get(int requestId, int userId)
        {
            User user = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            return await _service.GetByIdAsync(user.Id, requestId);
        }


        [HttpPost("/request")]
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> Post([FromBody] Request model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors));

            model.Type = (VacationType)model.TypeId;

            User user = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            model.UserId = user.Id;

            Request newRequest = await _service.AddAsync(model);

            return Ok(newRequest);
        }

        [HttpPut("/request/{requestId}")]
        public async Task Put(int requestId, [FromBody] Request newModel)
        {
            newModel.UserId = this.User.Identity.Name;
            await _service.UpdateAsync(requestId, newModel);
        }

        [Authorize(Roles = "Manager, Accountant, Admin")]
        [HttpDelete("/request/{requestId}")]
        public async Task Delete(int requestId)
        {
            await _service.DeleteAsync(requestId);
        }

        [HttpDelete("/user/request/{requestId}")]
        public async Task RejectByOwner(int requestId)
        {
            var userId = this.User.Identity.Name;
            await _service.RejectedByOwnerAsync(userId, requestId);
        }
    }


}
