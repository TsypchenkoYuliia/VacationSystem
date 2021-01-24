using Domain.DomainModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Context
{
    public class VacationSystemContext: IdentityDbContext<User>
    {
        public VacationSystemContext(DbContextOptions<VacationSystemContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UsedDaysStatistic> UsedDaysStatistics { get; set; }
    }
}
