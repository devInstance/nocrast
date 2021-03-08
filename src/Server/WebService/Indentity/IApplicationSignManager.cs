using Microsoft.AspNetCore.Identity;
using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Server.Indentity
{
    public interface IApplicationSignManager
    {
        Task<SignInResult> SignInAsync(ApplicationUser user, string password, bool persistent);
        Task SignOutAsync();
    }
}
