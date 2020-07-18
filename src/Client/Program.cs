using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NoCrast.Client.Services;
using System.Net.Http;
using NoCrast.Shared.Logging;
using Blazored.LocalStorage;
using NoCrast.Client.Services.Api;
using NoCrast.Client.Services.Net;
using NoCrast.Client.Services.LocalStore;
using NoCrast.Shared.Utils;

namespace NoCrast.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
            builder.Services.AddSingleton<ILogProvider>(new ConsoleLogProvider(LogLevel.DEBUG));

            builder.Services.AddScoped<IAuthorizationApi, AuthorizationApi>();
            builder.Services.AddScoped<ITasksApi, TasksApi>();

            builder.Services.AddScoped<IDataProvider, DataProvider>();

            builder.Services.AddScoped<IdentityAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<IdentityAuthenticationStateProvider>());

            builder.Services.AddScoped<AuthorizationService>();
            builder.Services.AddScoped<TasksService>();

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
