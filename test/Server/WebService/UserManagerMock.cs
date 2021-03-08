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

        public bool Succeeded { get; }

        public UserManagerMock(Guid userId, bool succeeded)
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
            Succeeded = succeeded;
        }

        public UserManagerMock(Guid userId)
            : this(userId, false)
        {
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override string GetUserId(ClaimsPrincipal principal)
        {
            return UserId.ToString();
        }

        class IdentityResultMock : IdentityResult
        {
            public IdentityResultMock(bool successed)
            {
                Succeeded = successed;
                if(!successed)
                {
                    //Errors = new List<IdentityError>() { new IdentityError { Code = 20, Description = "Test error" } };
                }
            }
        }
        public async override Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return new IdentityResultMock(Succeeded);
        }
        public async override Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return new ApplicationUser { 
                Id = UserId,
                UserName = userName
            };
        }

        public async override Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return new ApplicationUser
            {
                Id = UserId,
                UserName = userId
            };
        }
    }
}
