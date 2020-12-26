using BusinessLogic.Services;
using BusinessLogic.Services.Intarfaces;
using BusinessLogic.Settings;
using DataAccess.Context;
using DataAccess.Infrastructure;
using Domain.DomainModel;
using EmailTemplateRender.Infrastructure;
using EmailTemplateRender.Services;
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
            RazorConfiguration.ConfigureServices(services, configuration);

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRequestService, RequestService>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IReviewService, ReviewService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.Configure<SmtpSettings>(opt => configuration.GetSection("SmtpSettings").Bind(opt));
            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.Configure<UrlConfig>(opt => configuration.GetSection("UIConfig").Bind(opt));
        }
        public static async Task ConfigureIdentityInicializerAsync(IServiceProvider provider)
        {
            var userManager = provider.GetRequiredService<UserManager<User>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            await new IdentityInitializer(userManager, roleManager).SeedAsync();
        }
    }
}
