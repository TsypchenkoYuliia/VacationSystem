using BusinessLogic.Models;
using Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Intarfaces
{
    public interface IStatisticService
    {
        Task AddAsync(UsedDaysStatistic obj);
        Task UpdateAsync(int statisticId, UsedDaysStatistic obj);
        Task<IEnumerable<UsedDaysStatisticApiModel>> GetStatisticByUserAsync(string userId);
    }
}
