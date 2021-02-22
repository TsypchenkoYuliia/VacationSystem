using Domain.DomainModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context
{
    public class VacationSystemContext: IdentityDbContext<User>
    {
        public VacationSystemContext(DbContextOptions<VacationSystemContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UsedDaysStatistic> UsedDaysStatistics { get; set; }
    }
}
