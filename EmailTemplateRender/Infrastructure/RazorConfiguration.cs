using EmailTemplateRender.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


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
