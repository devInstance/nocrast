using Blazored.LocalStorage;
using Moq;
using NoCrast.Client.Services.Api;
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
        //[Fact()]
        //public async void GetTasksAsync_EmptyTest()
        //{
        //    var storage = TestUtils.CreateStorageProviderMock(null);
        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                TestUtils.CreateTasksApi());


        //    var result = await service.GetTasksAsync();

        //    Assert.Empty(result);
        //    Assert.True(false, "Add API call verification");
        //    storage.Verify(x => x.GetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName)), Times.Once());
        //    storage.Verify();
        //}

        //[Fact()]
        //public async void GetTasksAsync_MultiElementTest()
        //{
        //    var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.SyncUpWithServer(It.IsAny<TaskItem[]>())).Returns<TaskItem[]>((task) => Task.FromResult(task));

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                tasksApi.Object);

        //    var result = await service.GetTasksAsync();
        //    Assert.True(false, "Add API call verification");

        //    Assert.Equal(2, result.Count);
        //    Assert.Equal("Test 1", result[0].Task.Title);
        //    storage.Verify(x => x.GetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName)), Times.Once());
        //    storage.Verify();
        //}

        //[Fact()]
        //public async void AddNewTaskAsync_EmptyList()
        //{
        //    bool hasEventOccured = false;

        //    var storage = TestUtils.CreateStorageProviderMock(null);
        //    storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 1))).Returns(Task.FromResult(true));

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.AddTaskAsync(It.IsAny<TaskItem>())).Returns<TaskItem>((task) => Task.FromResult(task));

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                tasksApi.Object);
        //    service.DataHasChanged += delegate (object sender, EventArgs e)
        //    {
        //        hasEventOccured = true;
        //    };

        //    var result = await service.AddNewTaskAsync("Test 3");

        //    Assert.True(hasEventOccured);
        //    Assert.NotNull(result);
        //    Assert.Equal("Test 3", result.Task.Title);
        //    storage.Verify();
        //}

        //[Fact()]
        //public async void AddNewTaskAsync_NonEmptyList()
        //{
        //    bool hasEventOccured = false;

        //    var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
        //    storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 3))).Returns(Task.FromResult(true));

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.AddTaskAsync(It.IsAny<TaskItem>())).Returns<TaskItem>((task) => Task.FromResult(task));

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                tasksApi.Object);

        //    service.DataHasChanged += delegate (object sender, EventArgs e)
        //    {
        //        hasEventOccured = true;
        //    };

        //    var result = await service.AddNewTaskAsync("Test 3");

        //    Assert.True(hasEventOccured);
        //    Assert.NotNull(result);
        //    Assert.Equal("Test 3", result.Task.Title);
        //    storage.Verify();
        //}

        //[Theory]
        //[InlineData("  ")]
        //[InlineData("")]
        //[InlineData(null)]
        //public void AddNewTaskAsync_ParameterTest(string name)
        //{
        //    bool hasEventOccured = false;

        //    var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                TestUtils.CreateTasksApi());

        //    service.DataHasChanged += delegate (object sender, EventArgs e)
        //    {
        //        hasEventOccured = true;
        //    };

        //    Action actual = () => AsyncExceptionExtractor.Run(service.AddNewTaskAsync(name));
        //    Assert.Throws<ArgumentException>(actual);

        //    Assert.False(hasEventOccured);
        //    VerifySetItemNaverCalled(storage);
        //}

        //private static void VerifySetItemNaverCalled(Mock<ILocalStorageService> storage)
        //{
        //    storage.Verify(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.IsAny<NoCrastData>()), Times.Never());
        //}

        //[Fact()]
        //public async void AddNewTaskAsync_TitleAlreadyExists()
        //{
        //    bool hasEventOccured = false;
        //    ServiceErrorEventArgs error = null;

        //    var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.SyncUpWithServer(It.IsAny<TaskItem[]>())).Returns<TaskItem[]>((task) => Task.FromResult(task));

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                tasksApi.Object);

        //    service.DataHasChanged += delegate (object sender, EventArgs e)
        //    {
        //        hasEventOccured = true;
        //    };

        //    service.ErrorHasOccured += delegate (object sender, ServiceErrorEventArgs e)
        //    {
        //        error = e;
        //    };

        //    await service.AddNewTaskAsync("Test 1");

        //    Assert.False(hasEventOccured);
        //    Assert.NotNull(error);
        //    Assert.True(error.IsUIError);
        //    Assert.False(error.ResetUIError);
        //    VerifySetItemNaverCalled(storage);
        //    storage.Verify();
        //}

        [Fact()]
        public async void RemoveTaskAsync_NonEmptyList()
        {
            Assert.True(false, "Rewrite");
            //bool hasEventOccured = false;

            //var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            //storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 1 && d.Tasks[0].Title == "Test 2"))).Returns(Task.FromResult(true));

            //var tasksApi = new Mock<ITasksApi>();
            //tasksApi.Setup(x => x.SyncUpWithServer(It.IsAny<TaskItem[]>())).Returns<TaskItem[]>((task) => Task.FromResult(task));

            //TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
            //                                            TestUtils.CreateLogProvider(),
            //                                            storage.Object,
            //                                            tasksApi.Object);
            //service.DataHasChanged += delegate (object sender, EventArgs e)
            //{
            //    hasEventOccured = true;
            //};

            //var result = await service.RemoveTaskAsync(twoElementsData.Tasks[0]);

            //Assert.True(hasEventOccured);
            //storage.Verify();
        }

        [Fact()]
        public async void RemoveTaskAsync_EmptyList()
        {
            Assert.True(false, "Rewrite");
            //bool hasEventOccured = false;

            //var storage = TestUtils.CreateStorageProviderMock(null);

            //TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
            //                                            TestUtils.CreateLogProvider(),
            //                                            storage.Object,
            //                                            TestUtils.CreateTasksApi());
            //service.DataHasChanged += delegate (object sender, EventArgs e)
            //{
            //    hasEventOccured = true;
            //};

            //var result = await service.RemoveTaskAsync(twoElementsData.Tasks[0]);

            //Assert.False(hasEventOccured);
            //VerifySetItemNaverCalled(storage);
            //storage.Verify();
        }

        [Theory]
        [InlineData(null)]
        public async void RemoveTaskAsync_InvalidParameters(TaskItem item)
        {
            Assert.True(false, "Rewrite");
            //bool hasEventOccured = false;

            //var storage = TestUtils.CreateStorageProviderMock(twoElementsData);

            //TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
            //                                            TestUtils.CreateLogProvider(),
            //                                            storage.Object,
            //                                            TestUtils.CreateTasksApi());
            //service.DataHasChanged += delegate (object sender, EventArgs e)
            //{
            //    hasEventOccured = true;
            //};

            //var result = await service.RemoveTaskAsync(item);

            //Assert.False(hasEventOccured);
            //VerifySetItemNaverCalled(storage);
            //storage.Verify();
        }

        ///// <summary>
        ///// Case with previous network error
        ///// </summary>
        //[Fact()]
        //public void StartTaskAsync_TaskHasntBeenSynced()
        //{
        //    var requestTask = new TaskItem
        //    {
        //        IsRunning = false,
        //        TimeLogCount = 1,
        //        Title = "Test 1"
        //    };
        //    requestTask.ClientId = IdGenerator.New();

        //    var responseLog = new TimeLogItem
        //    {
        //        Id = "sdfjkhasd",
        //        ElapsedMilliseconds = 0,
        //        StartTime = DateTime.Now
        //    };

        //    var responseTask = new TaskItem
        //    {
        //        Id = "wquieyqwuiyre",
        //        IsRunning = true,
        //        TimeLogCount = 1,
        //        Title = "Test 1",
        //        ActiveTimeLogItemId = responseLog.Id
        //    };

        //    var data = new NoCrastData
        //    {
        //        Tasks = new List<TaskItem>
        //        {
        //            requestTask
        //        },
        //        Logs = new List<List<TimeLogItem>>
        //        {
        //            new List<TimeLogItem>()
        //        }
        //    };

        //    var itemView = new TaskItemView(TestUtils.CreateTimerProvider(), requestTask, null, 0);
        //    bool hasEventOccured = false;

        //    var storage = TestUtils.CreateStorageProviderMock(data);
        //    storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 2))).Returns(Task.FromResult(true));

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.SyncUpWithServer(It.IsAny<TaskItem[]>())).Returns<TaskItem[]>((task) => Task.FromResult(task));
        //    tasksApi.Setup(x => x.UpdateTimerAsync(It.Is<UpdateTaskParameters>(r  => r.Task.IsRunning))).Returns<UpdateTaskParameters>((req) => Task.FromResult(new UpdateTaskParameters
        //    {
        //        Task = responseTask,
        //        Log = responseLog
        //    })) ;

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                                TestUtils.CreateLogProvider(),
        //                                                storage.Object,
        //                                                tasksApi.Object);

        //    service.DataHasChanged += delegate (object sender, EventArgs e)
        //    {
        //        hasEventOccured = true;
        //    };

        //    service.StartTaskAsync(itemView);

        //    Assert.True(hasEventOccured, "hasEventOccured failed");
        //    Assert.True(data.Tasks[0].IsRunning, "Task is not running");
        //    Assert.Equal(responseTask.Id, data.Tasks[0].Id);
        //    Assert.Equal(responseLog.Id, data.Tasks[0].ActiveTimeLogItemId);
        //    Assert.Single(data.Logs[0]);
        //    Assert.Equal(responseLog.Id, data.Logs[0][0].Id);
        //    storage.Verify();
        //}

        [Fact()]
        public void StopTaskAsync_Success()
        {
            Assert.True(false, "Rewrite");
            //bool hasEventOccured = false;

            //var storage = TestUtils.CreateStorageProviderMock(twoElementsData);
            //storage.Setup(x => x.SetItemAsync<NoCrastData>(It.Is<string>(n => n == NoCrastData.StorageKeyName), It.Is<NoCrastData>(d => d.Tasks.Count == 2))).Returns(Task.FromResult(true));

            //var tasksApi = new Mock<ITasksApi>();
            //tasksApi.Setup(x => x.SyncUpWithServer(It.IsAny<TaskItem[]>())).Returns<TaskItem[]>((task) => Task.FromResult(task));

            //TimersService service = new TimersService(TestUtils.CreateTimerProvider(),
            //                                            TestUtils.CreateLogProvider(),
            //                                            storage.Object,
            //                                            tasksApi.Object);
            //service.DataHasChanged += delegate (object sender, EventArgs e)
            //{
            //    hasEventOccured = true;
            //};

            //service.StopTaskAsync(twoElementsData.Tasks[0]);

            //Assert.True(hasEventOccured);
            //Assert.False(twoElementsData.Tasks[0].IsRunning);
            //storage.Verify();
        }

        //[Fact()]
        //public async void ErrorHasOccured_NetworkError_AddNewTaskAsync()
        //{
        //    ServiceErrorEventArgs error = null;

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.GetTasksAsync()).Throws<Exception>();
        //    tasksApi.Setup(x => x.AddTaskAsync(It.IsAny<TaskItem>())).Throws<Exception>();

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                    TestUtils.CreateLogProvider(),
        //                                    TestUtils.CreateStorageProvider(null),
        //                                    tasksApi.Object);

        //    service.ErrorHasOccured += delegate (object sender, ServiceErrorEventArgs e)
        //    {
        //        error = e;
        //    };

        //    await service.AddNewTaskAsync("Test 1");

        //    Assert.NotNull(error);
        //    Assert.True(error.IsNetworkError);
        //    Assert.False(error.ResetNetworkError);
        //}

        //[Fact()]
        //public async void ErrorHasOccured_NetworkRecovery_AddNewTaskAsync()
        //{
        //    int counter = 0;
        //    ServiceErrorEventArgs error = null;

        //    var tasksApi = new Mock<ITasksApi>();
        //    tasksApi.Setup(x => x.GetTasksAsync()).Throws<Exception>();

        //    TasksService service = new TasksService(TestUtils.CreateTimerProvider(),
        //                                    TestUtils.CreateLogProvider(),
        //                                    TestUtils.CreateStorageProvider(null),
        //                                    tasksApi.Object);

        //    service.ErrorHasOccured += delegate (object sender, ServiceErrorEventArgs e)
        //    {
        //        counter++;
        //        error = e;
        //    };

        //    await service.AddNewTaskAsync("Test 1");

        //    Assert.NotNull(error);
        //    Assert.Equal(2, counter);
        //    Assert.False(error.IsNetworkError);
        //    Assert.True(error.ResetNetworkError);
        //}
    }
}
