using DataAccess.Context;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
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
            services.AddTransient<IRepository<Review, int>, ReviewRepository>();
            services.AddTransient<IRepository<User, string>, UserRepository>();
            services.AddTransient<IRepository<Request, int>, RequestRepository>();
            services.AddScoped<IRepository<UsedDaysStatistic, int>, StatisticRepository>();

            services.AddDbContext<VacationSystemContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("defaultConnection")));

            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<VacationSystemContext>()
               .AddDefaultTokenProviders();
        }
    }
}
