using Blazored.LocalStorage;
using Moq;
using NoCrast.Client.Model;
using NoCrast.Client.Services.Api;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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

        internal static Mock<ILocalStorageService> CreateStorageProviderMock(NoCrastData data)
        {
            var provider = new Mock<ILocalStorageService>();
            provider.Setup(x => x.GetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName))).Returns(Task.FromResult(data));
            return provider;
        }

        internal static ILocalStorageService CreateStorageProvider(NoCrastData data)
        {
            return CreateStorageProviderMock(data).Object;
        }

        internal static ITasksApi CreateTasksApi()
        {
            var tasksApi = new Mock<ITasksApi>();
            return tasksApi.Object;
        }

    }
}
