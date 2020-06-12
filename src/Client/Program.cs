using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NoCrast.Client.Services;
using NoCrast.Client.Utils;
using System.Net.Http;
using NoCrast.Shared.Logging;
using Blazored.LocalStorage;

namespace NoCrast.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
            builder.Services.AddSingleton<ILogProvider>(new ConsoleLogProvider(LogLevel.DEBUG));

            builder.Services.AddScoped<TimersService>();

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
