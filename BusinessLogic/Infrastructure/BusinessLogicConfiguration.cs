using BusinessLogic.Services;
using BusinessLogic.Services.Intarfaces;
using DataAccess.Context;
using DataAccess.Infrastructure;
using Domain.DomainModel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Infrastructure
{
    public static class BusinessLogicConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            DataAccessConfiguration.ConfigureServices(services, configuration);
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }
        public static async Task ConfigureIdentityInicializerAsync(IServiceProvider provider)
        {
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            await new IdentityInitializer(userManager, roleManager).SeedAsync();
        }
    }
}
