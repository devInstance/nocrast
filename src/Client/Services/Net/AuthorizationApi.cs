using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Net
{
    public class AuthorizationApi : ApiBase, IAuthorizationApi
    {
        private const string Controller = "api/user/account/";

        public AuthorizationApi(HttpClient http) : base (http)
        {
        }

        public async Task RegisterAsync(RegisterParameters registerParameters)
        {
            var result = await httpClient.PostAsJsonAsync(Controller + "register", registerParameters);
            result.EnsureSuccessStatusCode();
        }

        public async Task LoginAsync(LoginParameters loginParameters)
        {
            var result = await httpClient.PostAsJsonAsync(Controller + "signin", loginParameters);
            result.EnsureSuccessStatusCode();
        }

        public async Task LogoutAsync()
        {
            var result = await httpClient.PostAsync(Controller + "signout", null);
            result.EnsureSuccessStatusCode();
        }

        public async Task<UserInfoItem> GetUserInfoAsync()
        {
            return await httpClient.GetFromJsonAsync<UserInfoItem>(Controller + "user-info");
        }
    }
}
