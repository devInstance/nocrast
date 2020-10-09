using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services.Api
{
    public interface IUserProfileApi
    {
        Task<UserProfileItem> GetProfileAsync();

        Task<UserProfileItem> UpdateProfileAsync(UserProfileItem item);
    }
}
