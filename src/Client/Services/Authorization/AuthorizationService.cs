using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Authorization
{
    public class AuthorizationService
    {
        private const string Controller = "api/user/account/";

        private UserInfo currentUser;
        private readonly HttpClient httpClient;

        public AuthorizationService(HttpClient http)
        {
            httpClient = http;
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
            currentUser = null;
        }

        public async Task<UserInfo> GetUserInfoAsync()
        {
            if (currentUser != null && currentUser.IsAuthenticated)
            {
                return currentUser;
            }
            
            currentUser = await httpClient.GetFromJsonAsync<UserInfo>(Controller + "user-info");
            return currentUser;
        }
    }
}
