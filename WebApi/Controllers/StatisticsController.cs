using BusinessLogic.Models;
using BusinessLogic.Services.Intarfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : BaseController
    {
        private IStatisticService _service;

        public StatisticsController(IStatisticService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Manager, Employee")]
        [HttpGet]
        public async Task<IEnumerable<UsedDaysStatisticApiModel>> Get()
        {
            return await _service.GetStatisticByUserAsync(this.User.Identity.Name);
        }
    }
}
