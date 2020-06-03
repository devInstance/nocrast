using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Utils
{
    public interface IJsRuntime
    {
        ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object[] args);

        ValueTask InvokeVoidAsync(string identifier, params object[] args);
    }
}
