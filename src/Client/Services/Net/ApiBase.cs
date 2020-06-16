using System.Net.Http;

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
