using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{

    public interface IAuthorizationApi
    {
        Task RegisterAsync(RegisterParameters registerParameters);

        Task LoginAsync(LoginParameters loginParameters);

        Task LogoutAsync();

        Task<UserInfoItem> GetUserInfoAsync();
    }
}
