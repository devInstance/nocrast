using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NoCrast.Server.Database;
using NoCrast.Server.Model;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace NoCrast.ServerTests
{
    public class UserManagerMock : UserManager<ApplicationUser>
    {
        private Guid UserId;

        public UserManagerMock(Guid userId)
            : base(new Mock<IUserStore<ApplicationUser>>().Object,
          new Mock<IOptions<IdentityOptions>>().Object,
          new Mock<IPasswordHasher<ApplicationUser>>().Object,
          new IUserValidator<ApplicationUser>[0],
          new IPasswordValidator<ApplicationUser>[0],
          new Mock<ILookupNormalizer>().Object,
          new Mock<IdentityErrorDescriber>().Object,
          new Mock<IServiceProvider>().Object,
          new Mock<ILogger<UserManager<ApplicationUser>>>().Object/*,
                  new Mock<IHttpContextAccessor>().Object*/)
        {
            UserId = userId;
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override string GetUserId(ClaimsPrincipal principal)
        {
            return UserId.ToString();
        }

        public async override Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return new ApplicationUser { 
                Id = UserId,
                UserName = userName
            };
        }
    }
}
