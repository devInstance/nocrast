using NoCrast.Client.Model;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Storage
{
    public class LocalStorageProvider : IStorageProvider
    {
        private const string storageKeyName = "nocrast_data";
        private ILog Log { get; set; }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public LocalStorageProvider(ILogProvider log, IJsRuntime jsRuntime)
        {
            Log = log.CreateLogger(this);

            JsRuntime = jsRuntime;
            if (JsRuntime == null)
            {
                Log.D("JsRuntime is null");
            }
        }

        public IJsRuntime JsRuntime { get; }

        public async Task<NoCrastData> ReadAsync()
        {
            using (var s = Log.DebugScope())
            {
                var result = await JsRuntime.InvokeAsync<string>("localStorage.getItem", storageKeyName);
                if (result == null)
                {
                    return null;
                }
                return JsonSerializer.Deserialize<NoCrastData>(result, options);
            }
        }

        public async Task<bool> SaveAsync(NoCrastData value)
        {
            using (var s = Log.DebugScope())
            {
                string result = JsonSerializer.Serialize<NoCrastData>(value, options);
                Log.Line(LogLevel.DEBUG_EXTRA, $"SaveAsync: {result}");
                /*await*/
                JsRuntime.InvokeVoidAsync("localStorage.setItem", storageKeyName, result);
                return true;
            }
        }
    }
}
