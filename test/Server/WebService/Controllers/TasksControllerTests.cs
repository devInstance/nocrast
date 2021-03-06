﻿using Xunit;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Shared.Model;
using System;
using Moq;
using NoCrast.Shared.Utils;
using NoCrast.TestUtils;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class TasksControllerTests
    {
        long HOURS = 60 * 60 * 1000;

        [Fact()]
        public void GetTasksSimpleSuccessfulTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1").CreateTask("Task 2").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTasks(0, null, null, null);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.False(resultList[0].IsRunning);
                Assert.False(resultList[1].IsRunning);

                Assert.Equal(0, resultList[0].TimeLogCount);
                Assert.Equal(0, resultList[1].TimeLogCount);

                Assert.Equal(0, resultList[0].TotalTimeSpent);
                Assert.Equal(0, resultList[1].TotalTimeSpent);

                Assert.Equal(0, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(0, resultList[1].TotalTimeSpentThisWeek);

                Assert.Equal(0, resultList[0].TotalTimeSpentToday);
                Assert.Equal(0, resultList[1].TotalTimeSpentToday);
            }
        }

        [Fact()]
        public void GetTasksWithProjectSuccessfulTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("TTT", ProjectColor.Red).CreateTask("Task 1").CreateTask("Task 2").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTasks(0, null, null, null);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.NotNull(resultList[0].Project);
                Assert.Equal(ProjectColor.Red, resultList[0].Project.Color);

                Assert.NotNull(resultList[1].Project);
                Assert.Equal(ProjectColor.Red, resultList[1].Project.Color);
            }
        }

        [Fact()]
        public void GetTasksRunningOnlyTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);

            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, true)
                    .CreateTask("Task 2")
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTasks(0, null, null, TaskFilter.RunningOnly);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.True(resultList[0].IsRunning);
                Assert.Equal("Task 1", resultList[0].Title);
            }
        }

        [Fact()]
        public void GetTasksStoppedOnlyTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);

            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, true)
                    .CreateTask("Task 2")
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTasks(0, null, null, TaskFilter.StoppedOnly);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);
                Assert.Equal("Task 2", resultList[0].Title);
            }
        }

        [Fact()]
        public void GetTasksSimpleSuccessfulTop3Test()
        {
            var testTime = new DateTime(2020, 6, 7, 7, 0, 0);

            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.CurrentTime).Returns(testTime);

            using (TestDatabase db_test = new TestDatabase(timeProvider.Object))
            {
                var context = db_test.UserProfile().CreateTask("Task 1");
                testTime = testTime.AddMinutes(1);
                timeProvider.Setup(t => t.CurrentTime).Returns(testTime);
                context.CreateTask("Task 2");
                testTime = testTime.AddMinutes(1);
                timeProvider.Setup(t => t.CurrentTime).Returns(testTime);
                context.CreateTask("Task 3");
                testTime = testTime.AddMinutes(1);
                timeProvider.Setup(t => t.CurrentTime).Returns(testTime);
                context.CreateTask("Task 4");
                testTime = testTime.AddMinutes(1);
                timeProvider.Setup(t => t.CurrentTime).Returns(testTime);
                context.CreateTask("Task 5");
                context.EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider.Object);

                var result = controller.GetTasks(0, 3, 0, null);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(3, resultList.Length);
                Assert.Equal("Task 5", resultList[0].Title);
                Assert.Equal(testTime, resultList[0].UpdateDate);

                Assert.Equal("Task 4", resultList[1].Title);
                Assert.Equal("Task 3", resultList[2].Title);

                result = controller.GetTasks(0, 3, 1, null);

                Assert.True(result.Result is OkObjectResult);
                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Task 2", resultList[0].Title);
                Assert.Equal("Task 1", resultList[1].Title);
            }
        }

        [Fact()]
        public void GetTasksTimeOffsetTodayTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(0, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(8 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(8 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+7
                result = controller.GetTasks(7 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(10 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(10 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+11
                result = controller.GetTasks(11 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC-7
                result = controller.GetTasks(-7 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);
            }
        }

        [Fact()]
        public void GetTasksTimeOffsetTomorrowTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time.AddDays(1));
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(0, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(8 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(0 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+7
                result = controller.GetTasks(7 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(10 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(0 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+11
                result = controller.GetTasks(11 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(2 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC-7
                result = controller.GetTasks(-7 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(4 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(4 * HOURS, resultList[0].TotalTimeSpentToday);
            }
        }

        [Fact()]
        public void GetTasksTimeOffsetYesterdayTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time.AddDays(-1));
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(0, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+7
                result = controller.GetTasks(7 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+11
                result = controller.GetTasks(11 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC-7
                result = controller.GetTasks(-7 * 60, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);
            }
        }

        [Fact()]
        public void GetTasksTimeOffsetMyTestTest()
        {
            var time = new DateTime(2020, 8, 18, 18, 35, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(new DateTime(2020, 8, 18, 16, 35, 0), 1 * HOURS, false)
                    .CreateTimeLog(new DateTime(2020, 8, 17, 22, 59, 0), 1 * HOURS, false)
                    .CreateTimeLog(new DateTime(2020, 8, 17, 21, 31, 0), 1 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(-420, null, null, null);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(3, resultList[0].TimeLogCount);

                Assert.Equal(3 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(3 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(1 * HOURS, resultList[0].TotalTimeSpentToday);

            }
        }

        [Fact()]
        public void AddTaskTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, TimerProviderMock.CreateTimerProvider());

                var input = new TaskItem
                {
                    ClientId = "clintid_check",
                    Title = "Test 1"
                };
                var result = controller.AddTask(input);

                Assert.True(result.Result is OkObjectResult);
                var resTask = ((TaskItem)((OkObjectResult)result.Result).Value);
                Assert.Equal("clintid_check", resTask.ClientId);
                Assert.Equal("Test 1", resTask.Title);

                var task = db_test.FetchTask(resTask.Id);
                Assert.Equal("Test 1", task.Title);

                //TODO: verify status
            }
        }

        [Fact()]
        public void RemoveProjectTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("P").CreateTask("Task")
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, TimerProviderMock.CreateTimerProvider());

                var input = new TaskItem
                {
                    Id = db_test.lastTask.PublicId,
                    Title = "Test 1",
                    Project = null
                };
                var result = controller.UpdateTask(input.Id, input, 0);

                Assert.True(result.Result is OkObjectResult);
                var resTask = ((TaskItem)((OkObjectResult)result.Result).Value);
                Assert.Equal("Test 1", resTask.Title);
                Assert.Null(resTask.Project);

                var task = db_test.FetchTask(resTask.Id);
                Assert.Equal("Test 1", task.Title);
                Assert.Null(task.Project);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnAllTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddDays(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, null, null, null);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(6, resultList.Items.Length);
                Assert.Equal(6, resultList.TotalCount);
                Assert.Equal(6, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(1, resultList.PagesCount);
                Assert.Equal(time.AddDays(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTodayTest()
        {
            var time = new DateTime(2020, 8, 12, 15, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-24f * 2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, null, null, TimeLogResultType.Day);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(3, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(3, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(1, resultList.PagesCount);
                Assert.Equal(time.AddHours(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnWeekTest()
        {
            var time = new DateTime(2020, 8, 12, 15, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-24f * 2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, null, null, TimeLogResultType.Week);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(5, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(5, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(1, resultList.PagesCount);
                Assert.Equal(time.AddHours(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTop5Test()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-24f * 2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, 5, null, TimeLogResultType.All);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(5, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(9, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(2, resultList.PagesCount);
                Assert.Equal(time.AddHours(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTop5Page2Test()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-24f * 2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, 5, 1, TimeLogResultType.All);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(4, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(9, resultList.Count);
                Assert.Equal(1, resultList.Page);
                Assert.Equal(2, resultList.PagesCount);
                Assert.Equal(time.AddDays(-4f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTop3PageCount4Test()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-24f * 2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-1f), 1 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, 3, 1, TimeLogResultType.All);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(3, resultList.Items.Length);
                Assert.Equal(10, resultList.TotalCount);
                Assert.Equal(10, resultList.Count);
                Assert.Equal(1, resultList.Page);
                Assert.Equal(4, resultList.PagesCount);
                Assert.Equal(time.AddDays(-1f), resultList.Items[0].StartTime);
            }
        }

        [Theory]
        [InlineData(0, 5, TimeLogResultType.All, 10, 5, 0, 2)]
        [InlineData(1, 5, TimeLogResultType.All, 10, 5, 1, 2)]
        [InlineData(2, 5, TimeLogResultType.All, 10, 5, 1, 2)]
        [InlineData(3, 5, TimeLogResultType.All, 10, 5, 1, 2)]
        [InlineData(0, 3, TimeLogResultType.All, 10, 3, 0, 4)]
        [InlineData(1, 3, TimeLogResultType.All, 10, 3, 1, 4)]
        [InlineData(2, 3, TimeLogResultType.All, 10, 3, 2, 4)]
        [InlineData(3, 3, TimeLogResultType.All, 10, 1, 3, 4)]
        [InlineData(4, 3, TimeLogResultType.All, 10, 1, 3, 4)]
        [InlineData(5, 3, TimeLogResultType.All, 10, 1, 3, 4)]
        [InlineData(-5, 3, TimeLogResultType.All, 10, 3, 0, 4)]
        [InlineData(1, -3, TimeLogResultType.All, 10, 10, 0, 1)]
        [InlineData(-1, -3, TimeLogResultType.All, 10, 10, 0, 1)]
        [InlineData(0, 5, TimeLogResultType.Day, 3, 3, 0, 1)]
        [InlineData(0, 5, TimeLogResultType.Week, 6, 5, 0, 2)]
        [InlineData(0, 2, TimeLogResultType.Week, 6, 2, 0, 3)]
        public void GetTimelogAsyncReturnTopPageSizeTest(int? page, int? top, TimeLogResultType? type, int expectedQueryCount, int expectedItemsCount, int expectedPage, int expectedPagesCount)
        {
            var time = new DateTime(2020, 8, 12, 15, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-24f * 2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-1f), 1 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, top, page, type);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(expectedItemsCount, resultList.Items.Length);
                Assert.Equal(10, resultList.TotalCount);
                Assert.Equal(expectedQueryCount, resultList.Count);
                Assert.Equal(expectedPage, resultList.Page);
                Assert.Equal(expectedPagesCount, resultList.PagesCount);
            }
        }

        [Fact()]
        public void DeleteTimerLogAsyncNullActiveLogItemTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, false).
                    EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                db_test.SetupDBContext();

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var lastLogId = db_test.lastLog.PublicId;
                var result = controller.DeleteTimerLogAsync(db_test.lastTask.PublicId, lastLogId, 0);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem)((OkObjectResult)result.Result).Value);
                Assert.Equal(0, resultList.TimeLogCount);
                Assert.Equal(0, resultList.TotalTimeSpent);
                Assert.Equal(0, resultList.TotalTimeSpentThisWeek);
                Assert.Equal(0, resultList.TotalTimeSpentToday);

                Assert.Null(db_test.FetchTimeLog(lastLogId));
            }
        }


        [Fact()]
        public void DeleteTimerLogAsyncActiveLogItemTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 2 * HOURS, true).
                    EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                db_test.SetupDBContext();

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var lastLogId = db_test.lastLog.PublicId;
                var result = controller.DeleteTimerLogAsync(db_test.lastTask.PublicId, lastLogId, 0);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem)((OkObjectResult)result.Result).Value);
                Assert.Equal(0, resultList.TimeLogCount);
                Assert.Equal(0, resultList.TotalTimeSpent);
                Assert.Equal(0, resultList.TotalTimeSpentThisWeek);
                Assert.Equal(0, resultList.TotalTimeSpentToday);

                Assert.Null(db_test.FetchTimeLog(lastLogId));
            }
        }

        [Fact()]
        public void DeleteTimerLogAsyncNonActiveLogItemTest()
        {
            var time = new DateTime(2020, 8, 12, 12, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-2f), 1 * HOURS, true);
                var lastLogId = db_test.lastLog.PublicId;
                db_test.CreateTimeLog(time.AddHours(-4f), 2 * HOURS, true)
                    .CreateTimeLog(time.AddHours(-6f), 2 * HOURS, true)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                db_test.SetupDBContext();

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.DeleteTimerLogAsync(db_test.lastTask.PublicId, lastLogId, 0);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TaskItem)((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.TimeLogCount);
                Assert.Equal(4 * HOURS, resultList.TotalTimeSpent);
                Assert.Equal(4 * HOURS, resultList.TotalTimeSpentThisWeek);
                Assert.Equal(4 * HOURS, resultList.TotalTimeSpentToday);

                Assert.Null(db_test.FetchTimeLog(lastLogId));
            }
        }
    }
}