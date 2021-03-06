﻿using BusinessLogic.Services.Intarfaces;
using Domain.DomainModel;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Authorize(Roles = "Manager, Employee")]
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

            model.CreatedDate = DateTime.Now.Date;

            Request newRequest = await _service.AddAsync(model);

            _logger.LogInformation($"Request created successfully(id: {newRequest.Id}, author: {model.UserId})");

            return Ok(newRequest);
        }

        [Authorize(Roles = "Manager, Employee")]
        [HttpPut("/request/{requestId}")]
        public async Task Put(int requestId, [FromBody] Request newModel)
        {
            User user = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            newModel.UserId = user.Id;
            await _service.UpdateAsync(requestId, newModel);
            _logger.LogInformation($"Request updated successfully(id: {requestId})");
        }

        [Authorize(Roles = "Manager, Accountant, Admin")]
        [HttpDelete("/request/{requestId}")]
        public async Task Delete(int requestId)
        {
            await _service.DeleteAsync(requestId);
            _logger.LogInformation($"Request deleted successfully(id: {requestId})");
        }

        [HttpDelete("/user/request/{requestId}")]
        public async Task RejectByOwner(int requestId)
        {
            User user = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            await _service.RejectedByOwnerAsync(user.Id, requestId);
            _logger.LogInformation($"Request rejected by author (id: {requestId}, authorId: {user.Id})");
        }
    }


}
