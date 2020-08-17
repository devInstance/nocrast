using Blazored.LocalStorage;
using Moq;
using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Utils;
using System;
using System.Threading.Tasks;

namespace NoCrast.ClientTests
{
    public static class TestUtils
    {
        internal static ITimeProvider CreateTimerProvider()
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.CurrentTime).Returns(new DateTime(2020, 6, 7, 7, 0, 0));
            return timeProvider.Object;
        }

        internal static ILogProvider CreateLogProvider()
        {
            var log = new Mock<ILogProvider>();
            log.Setup(
                x => x.CreateLogger(It.IsAny<string>()))
                        .Returns(new Mock<ILog>().Object);
            return log.Object;
        }

        internal static ITasksApi CreateTasksApi()
        {
            var tasksApi = new Mock<ITasksApi>();
            return tasksApi.Object;
        }

    }
}
