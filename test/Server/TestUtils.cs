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
    public class TestUtils
    {
        public static DateTime TEST_TIME = new DateTime(2020, 6, 7, 7, 0, 0);

        public static ITimeProvider CreateTimerProvider()
        {
            return CreateTimerProvider(TEST_TIME);
        }
        public static ITimeProvider CreateTimerProvider(DateTime time)
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.CurrentTime).Returns(time);
            return timeProvider.Object;
        }

        public static Mock<IApplicationSignManager> CreateSignManager(bool succeeded)
        {
            var signinManagerMq = new Mock<IApplicationSignManager>();
            signinManagerMq.Setup(x => x.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns<ApplicationUser, string, bool>((x, y, z) => Task.FromResult((Microsoft.AspNetCore.Identity.SignInResult)new SignInResultMock(succeeded)));

            return signinManagerMq;
        }
    }
}
