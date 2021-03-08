using DevInstance.LogScope;
using Moq;
using NoCrast.Server.Indentity;
using NoCrast.Server.Model;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NoCrast.ServerTests
{
    public class ServerTestUtils
    {

        public static Mock<IApplicationSignManager> CreateSignManager(bool succeeded)
        {
            var signinManagerMq = new Mock<IApplicationSignManager>();
            signinManagerMq.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns<ApplicationUser, string, bool>((x, y, z) => Task.FromResult((Microsoft.AspNetCore.Identity.SignInResult)new SignInResultMock(succeeded)));

            return signinManagerMq;
        }
    }
}
