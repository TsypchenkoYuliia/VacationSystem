using BusinessLogic.Services.Intarfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ReviewController(IReviewService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [Authorize(Roles = "Manager, Accountant")]
        [HttpGet("/user/reviews")]
        public async Task<IEnumerable<Review>> Get(int? requestId = null, int? stateId = null, DateTime? startDate = null, DateTime? endDate = null, string name = null, int? typeId = null)
        {
            var reviewer = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);

            return await _service.GetAllAsync(reviewer.Id, requestId, stateId, startDate, endDate, name, typeId); ;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/reviews")]
        public async Task<IEnumerable<Review>> Get(string reviewerId = null, int? requestId = null, int? stateId = null, DateTime? startDate = null, DateTime? endDate = null, string name = null, int? typeId = null)
        {
            return await _service.GetAllAsync(reviewerId, requestId, stateId, startDate, endDate, name, typeId); ;
        }

        [Authorize(Roles = "Manager, Accountant")]
        [HttpPut("/user/reviews/{id}")]
        public async Task Put(int id, [FromBody] Review model)
        {
            var reviewer = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);

            await _service.UpdateAsync(id, model, reviewer.Id);
        }


        [HttpDelete("/reviews/{id}")]
        public async Task Delete(int id)
        {
            await _service.DeleteAsync(id);
        }
    }
}
