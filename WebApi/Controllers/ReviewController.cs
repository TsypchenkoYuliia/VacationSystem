using BusinessLogic.Services.Intarfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
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
    public class ReviewController : ControllerBase
    {
        private IReviewService _service;
        private readonly IUserService _userService;
        private ILogger<ReviewController> _logger;
        public ReviewController(IReviewService service, IUserService userService, ILogger<ReviewController> logger)
        {
            _service = service;
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = "Manager, Accountant")]
        [HttpGet("/user/reviews")]
        public async Task<IEnumerable<Review>> Get(int? requestId = null, int? stateId = null, DateTime? startDate = null, DateTime? endDate = null, string name = null, int? typeId = null)
        {
            var reviewer = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            return (await _service.GetAllAsync(reviewer.Id, requestId, stateId, startDate, endDate, name, typeId)).OrderByDescending(x=>x.Request.CreatedDate.Date);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/reviews")]
        public async Task<IEnumerable<Review>> Get(string reviewerId = null, int? requestId = null, int? stateId = null, DateTime? startDate = null, DateTime? endDate = null, string name = null, int? typeId = null)
        {
            return (await _service.GetAllAsync(reviewerId, requestId, stateId, startDate, endDate, name, typeId)).OrderByDescending(x => x.Request.CreatedDate.Date);
        }

        [Authorize(Roles = "Manager, Accountant")]
        [HttpPut("/user/reviews/{id}")]
        public async Task Put(int id, [FromBody] Review model)
        {
            var reviewer = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);

            await _service.UpdateAsync(id, model, reviewer.Id);
            _logger.LogInformation($"Review updated successfully (id: {id}, state: {model.IsApproved})");
        }


        [HttpDelete("/reviews/{id}")]
        public async Task Delete(int id)
        {
            await _service.DeleteAsync(id);
            _logger.LogInformation($"Review deleted successfully (id: {id})");
        }
    }
}
