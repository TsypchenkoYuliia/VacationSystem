using EmailTemplateRender.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailTemplateRender.Infrastructure
{
    public static class RazorConfiguration
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRazorPages();

            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        }
    }
}
