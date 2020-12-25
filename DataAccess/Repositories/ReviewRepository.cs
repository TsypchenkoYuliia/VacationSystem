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
    public class ReviewRepository : BaseRepository<Review, int>
    {
        public ReviewRepository(VacationSystemContext context) : base(context) { }

        public override async Task<IReadOnlyCollection<Review>> FilterAsync(Expression<Func<Review, bool>> predicate)
        {
            return await Entities.Where(predicate)
                 .Include(r => r.Request)
                 .ThenInclude(u => u.Reviews)
                 .ToListAsync();
        }

        public override async Task<Review> FindAsync(Expression<Func<Review, bool>> predicate)
        {
            return await Entities.Where(predicate)
                 .Include(r => r.Request)
                 .ThenInclude(u => u.Reviews)
                 .FirstOrDefaultAsync();
        }
    }
}
