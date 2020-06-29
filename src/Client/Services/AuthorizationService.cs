using NoCrast.Client.Services.Api;
using NoCrast.Shared.Model;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class AuthorizationService
    {
        protected IAuthorizationApi Api { get; }

        private UserInfoItem currentUser;

        public AuthorizationService(IAuthorizationApi api)
        {
            Api = api;
        }

        public Task RegisterAsync(RegisterParameters registerParameters)
        {
            return Api.RegisterAsync(registerParameters);
        }

        public Task LoginAsync(LoginParameters loginParameters)
        {
            return Api.LoginAsync(loginParameters);
        }

        public Task LogoutAsync()
        {
            var result = Api.LogoutAsync();
            currentUser = null;
            return result;
        }

        public async Task<UserInfoItem> GetUserInfoAsync()
        {
            if (currentUser != null && currentUser.IsAuthenticated)
            {
                return currentUser;
            }

            currentUser = await Api.GetUserInfoAsync();
            return currentUser;
        }
    }
}
