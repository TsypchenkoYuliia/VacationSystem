using DataAccess.Context;
using Domain.DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Infrastructure
{
    public static class DataAccessConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<VacationSystemContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("defaultConnection")));

            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<VacationSystemContext>()
               .AddDefaultTokenProviders();
        }
    }
}
