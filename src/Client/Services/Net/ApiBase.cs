using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Net
{
    public class ApiBase
    {
        protected readonly HttpClient httpClient;
        public ApiBase(HttpClient http)
        {
            httpClient = http;
        }

    }
}
