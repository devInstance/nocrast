using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoCrast.Server.Database.Postgres
{
    public static class ConfigurationExtensions
    {
        public static void ConfigurePostgresDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext, PostgresApplicationDbContext>(options =>
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            //services.AddScoped<ApplicationDbContext>(services.Db)
        }
    }
}
