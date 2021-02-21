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
    public class RequestRepository : BaseRepository<Request, int>
    {
        public RequestRepository(VacationSystemContext context) : base(context) { }

        public async override Task<Request> FindAsync(Expression<Func<Request, bool>> predicate)
        {
            return await Entities.Where(predicate)
                 .Include(r => r.Reviews).ThenInclude(x => x.Reviewer)
                 .Include(u=>u.User)
                 .FirstOrDefaultAsync();
        }

        public async override Task<IReadOnlyCollection<Request>> FilterAsync(Expression<Func<Request, bool>> predicate)
        {
            return await Entities.Where(predicate)
                 .Include(r => r.Reviews).ThenInclude(x=>x.Reviewer)
                 .Include(u => u.User)
                 .ToListAsync();
        }
    }
}
