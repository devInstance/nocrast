using Blazored.LocalStorage;
using Moq;
using NoCrast.Client.Model;
using NoCrast.ClientTests;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NoCrast.Client.Services.Tests
{
    public class TimersServiceTests
    {
        private readonly NoCrastData twoElementsData = new NoCrastData
        {
            Tasks = new List<TaskItem>
                {
                    new TaskItem { Title = "Test 1" },
                    new TaskItem { Title = "Test 2" }
                }
        };

        [Fact()]
        public async void GetTasksAsync_EmptyTest()
        {
            var storage = TestUtils.CreateStorageProviderMock(null);
            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);


            var result = await service.GetTasksAsync();

            Assert.Empty(result);
            storage.Verify(x => x.GetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName)), Times.Once());
            storage.Verify();
        }

        [Fact()]
        public async void GetTasksAsync_MultiElementTest()
        {
            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);

            var result = await service.GetTasksAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal("Test 1", result[0].Title);
            storage.Verify(x => x.GetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName)), Times.Once());
            storage.Verify();
        }

        [Fact()]
        public async void AddNewTaskAsync_EmptyList()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(null);
            storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 1))).Returns(Task.FromResult(true));

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);
            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            var result = await service.AddNewTaskAsync("Test 3");

            Assert.True(hasEventOccured);
            Assert.Equal("Test 3", result.Title);
            storage.Verify();
        }

        [Fact()]
        public async void AddNewTaskAsync_NonEmptyList()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 3))).Returns(Task.FromResult(true));

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);

            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            var result = await service.AddNewTaskAsync("Test 3");

            Assert.True(hasEventOccured);
            Assert.Equal("Test 3", result.Title);
            storage.Verify();
        }

        [Theory]
        [InlineData("  ")]
        [InlineData("")]
        [InlineData(null)]
        public void AddNewTaskAsync_ParameterTest(string name)
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);

            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            Action actual = () => AsyncExceptionExtractor.Run(service.AddNewTaskAsync(name));
            Assert.Throws<ArgumentException>(actual);

            Assert.False(hasEventOccured);
            VerifySetItemNaverCalled(storage);
        }

        private static void VerifySetItemNaverCalled(Mock<ILocalStorageService> storage)
        {
            storage.Verify(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.IsAny<NoCrastData>()), Times.Never());
        }

        [Fact()]
        public void AddNewTaskAsync_TitleAlreadyExists()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);

            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            Action actual = () => AsyncExceptionExtractor.Run(service.AddNewTaskAsync("Test 1"));
            Assert.Throws<ArgumentException>(actual);

            Assert.False(hasEventOccured);
            VerifySetItemNaverCalled(storage);
            storage.Verify();
        }

        [Fact()]
        public async void RemoveTaskAsync_NonEmptyList()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 1 && d.Tasks[0].Title == "Test 2"))).Returns(Task.FromResult(true));

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);
            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            var result = await service.RemoveTaskAsync(twoElementsData.Tasks[0]);

            Assert.True(hasEventOccured);
            storage.Verify();
        }

        [Fact()]
        public async void RemoveTaskAsync_EmptyList()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(null);

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);
            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            var result = await service.RemoveTaskAsync(twoElementsData.Tasks[0]);

            Assert.False(hasEventOccured);
            VerifySetItemNaverCalled(storage);
            storage.Verify();
        }

        [Theory]
        [InlineData(null)]
        public async void RemoveTaskAsync_InvalidParameters(TaskItem item)
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);
            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            var result = await service.RemoveTaskAsync(item);

            Assert.False(hasEventOccured);
            VerifySetItemNaverCalled(storage);
            storage.Verify();
        }

        [Fact()]
        public void StartTaskAsync_Success()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 2))).Returns(Task.FromResult(true));

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);
            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            service.StartTaskAsync(twoElementsData.Tasks[0]);

            Assert.True(hasEventOccured);
            Assert.True(twoElementsData.Tasks[0].IsRunning);
            storage.Verify();
        }

        [Fact()]
        public void StopTaskAsync_Success()
        {
            bool hasEventOccured = false;

            var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 2))).Returns(Task.FromResult(true));

            TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
                                                        TestUtils.CreateLogProvider(),
                                                        storage.Object);
            service.DataHasChanged += delegate (object sender, EventArgs e)
            {
                hasEventOccured = true;
            };

            service.StopTaskAsync(twoElementsData.Tasks[0]);

            Assert.True(hasEventOccured);
            Assert.False(twoElementsData.Tasks[0].IsRunning);
            storage.Verify();
        }

    }
}
