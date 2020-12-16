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
    }
}
