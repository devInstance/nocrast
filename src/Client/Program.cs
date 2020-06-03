using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NoCrast.Client.Services;
using NoCrast.Client.Utils;
using System.Net.Http;
using NoCrast.Shared.Logging;
using System.Threading;
using NoCrast.Client.Storage;

namespace NoCrast.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
            builder.Services.AddSingleton<ILogProvider>(new ConsoleLogProvider(LogLevel.DEBUG));
            builder.Services.AddSingleton<IJsRuntime, JsRuntime>();
            builder.Services.AddSingleton<IStorageProvider, LocalStorageProvider>();

            builder.Services.AddSingleton<TimersService>();

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
