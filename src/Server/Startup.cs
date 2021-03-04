using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoCrast.Server.Database;
using NoCrast.Server.Indentity;
using NoCrast.Shared.Utils;
using NoCrast.Server.Services;
using DevInstance.LogScope;
using DevInstance.LogScope.Extensions;
using DevInstance.LogScope.Formaters;
using NoCrast.Server.Data.Queries.Postgres;
using NoCrast.Server.Data;

namespace NoCrast.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsoleLogging(LogLevel.INFO,
                new DefaultFormaterOptions { ShowTimestamp = true, ShowThreadNumber = true });

            services.ConfigureDatabase(Configuration);
            services.ConfigureIdentity();

            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddScoped<IApplicationSignManager, ApplicationSignManager>();

            services.AddScoped<IQueryRepository, PostgresQueryRepository>();

            services.AddScoped<ActivityReportService>();

            services.AddControllersWithViews().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
