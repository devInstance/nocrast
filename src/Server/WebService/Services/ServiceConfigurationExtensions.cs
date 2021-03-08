using Microsoft.Extensions.DependencyInjection;

namespace NoCrast.Server.Services
{
    public static class ServiceConfigurationExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ActivityReportService>();
            services.AddScoped<AggregateReportService>();
        }
    }
}
