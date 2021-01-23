using DevInstance.LogScope;
using Microsoft.AspNetCore.Components.Authorization;
using NoCrast.Shared.Model;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILog log;
        private readonly AuthorizationService service;

        public IdentityAuthenticationStateProvider(ILogProvider l, AuthorizationService s)
        {
            log = l.CreateLogger(this);
            service = s;
        }

        public async Task LoginAsync(LoginParameters loginParameters)
        {
            await service.LoginAsync(loginParameters);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task RegisterAsync(RegisterParameters registerParameters)
        {
            await service.RegisterAsync(registerParameters);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task LogoutAsync()
        {
            await service.LogoutAsync();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task DeleteAsync()
        {
            if (await service.DeleteAsync())
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();
            try
            {
                var userInfo = await service.GetUserInfoAsync();
                if (userInfo.IsAuthenticated)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, userInfo.UserName) }.Concat(userInfo.ExposedClaims.Select(c => new Claim(c.Key, c.Value)));
                    identity = new ClaimsIdentity(claims, "Server authentication");
                }
            }
            catch (HttpRequestException ex)
            {
                log.E("Request failed", ex);
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
