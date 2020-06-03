using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace NoCrast.Client.Utils
{
    public class JsRuntime : IJsRuntime
    {
        public IJSRuntime Runtime { get; }

        public JsRuntime(IJSRuntime runtime)
        {
            Runtime = runtime;
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object[] args)
        {
            return Runtime.InvokeAsync<TValue>(identifier, args);
        }

        public ValueTask InvokeVoidAsync(string identifier, params object[] args)
        {
            return Runtime.InvokeVoidAsync(identifier, args);
        }
    }
}
