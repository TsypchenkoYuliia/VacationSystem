using BusinessLogic.Models;
using BusinessLogic.Services.Intarfaces;
using Domain.DomainModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : BaseController
    {
        private IStatisticService _service;
        private readonly IUserService _userService;
        public StatisticsController(IStatisticService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [Authorize(Roles = "Manager, Employee")]
        [HttpGet]
        public async Task<IEnumerable<UsedDaysStatisticApiModel>> Get()
        {
            User user = await _userService.GetUser(x => x.UserName == this.User.Identity.Name);
            return (await _service.GetStatisticByUserAsync(user.Id)).Where(x=>x.Days > 0);
        }

        [Authorize(Roles = "Manager, Accountant")]
        [HttpGet("{userId}")]
        public async Task<IEnumerable<UsedDaysStatisticApiModel>> Get(string userId)
        {
            var res = await _service.GetStatisticByUserAsync(userId);
            return (await _service.GetStatisticByUserAsync(userId)).Where(x => x.Days > 0);
        }
    }
}
