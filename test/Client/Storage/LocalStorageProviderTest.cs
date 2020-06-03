using Moq;
using NoCrast.Client.Model;
using NoCrast.Client.Utils;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NoCrast.Client.Storage.Tests
{
    public class LocalStorageProviderTest
    {
        [Fact()]
        public async void ReadAsync_EmptyStorage_ReturnsNull()
        {
            LocalStorageProvider storage = SetupStorage();

            var res = await storage.ReadAsync();

            Assert.Null(res);
        }

        private static LocalStorageProvider SetupStorage()
        {
            var jsRuntime = new Mock<IJsRuntime>();
            return SetupStorage(jsRuntime);
        }

        private static LocalStorageProvider SetupStorage(Mock<IJsRuntime> jsRuntime)
        {
            var log = new Mock<ILogProvider>();
            log.Setup(
                x => x.CreateLogger(It.IsAny<string>()))
                        .Returns(new Mock<ILog>().Object);
            var storage = new LocalStorageProvider(log.Object, jsRuntime.Object);
            return storage;
        }

        [Fact()]
        public async void ReadAsync_NonEmptyStorage_ReturnsObject()
        {
            string data = "{\"tasks\": [{\"title\": \"Test 1\"}]}";
            var jsRuntime = new Mock<IJsRuntime>();
            jsRuntime.Setup(
                x => x.InvokeAsync<string>("localStorage.getItem", It.IsAny<string>()))
                        .Returns(new ValueTask<string>(Task.FromResult(data)));

            var storage = SetupStorage(jsRuntime);

            var res = await storage.ReadAsync();

            jsRuntime.Verify();
            Assert.Single(res.Tasks);
        }

        [Fact()]
        public async void SaveAsync_NonEmptyStorage_Success()
        {
            var storage = SetupStorage();
            var data = new NoCrastData
            {
                Tasks = new List<TaskItem>
                {
                    new TaskItem { Title = "Test 1" },
                    new TaskItem { Title = "Test 2" }
                }
            };
            var res = await storage.SaveAsync(data);

            Assert.True(res);
        }
    }
}
