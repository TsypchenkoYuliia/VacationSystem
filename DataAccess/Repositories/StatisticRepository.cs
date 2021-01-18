using DataAccess.Context;
using DataAccess.Repositories.Interfaces;
using Domain.DomainModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class StatisticRepository : BaseRepository<UsedDaysStatistic, int>
    {
        public StatisticRepository(VacationSystemContext context) : base(context) { }

        public async override Task<IReadOnlyCollection<UsedDaysStatistic>> FilterAsync(Expression<Func<UsedDaysStatistic, bool>> predicate)
        {
            return await Entities.Where(predicate)
                 .Include(r => r.Request)
                 .ToListAsync();
        }
    }
}
