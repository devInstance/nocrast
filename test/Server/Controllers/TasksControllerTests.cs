﻿using NoCrast.Server.Controllers;
using Xunit;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Shared.Model;
using System;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class TasksControllerTests
    {
        [Fact()]
        public void GetTasksSimpleSuccessfulTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1").CreateTask("Task 2");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTasks(0);

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

        long HOURS = 60 * 60 * 1000;

        [Fact()]
        public void GetTasksTimeOffsetTodayTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(0);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(8 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(8 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+7
                result = controller.GetTasks(7 * 60);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(10 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(10 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+11
                result = controller.GetTasks(11 * 60);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC-7
                result = controller.GetTasks(-7 * 60);

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
            var timeProvider = TestUtils.CreateTimerProvider(time.AddDays(1));
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(0);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(8 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(0 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+7
                result = controller.GetTasks(7 * 60);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(10 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(0 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+11
                result = controller.GetTasks(11 * 60);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(2 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC-7
                result = controller.GetTasks(-7 * 60);

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
            var timeProvider = TestUtils.CreateTimerProvider(time.AddDays(-1));
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(0);

                Assert.True(result.Result is OkObjectResult);

                var resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+7
                result = controller.GetTasks(7 * 60);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC+11
                result = controller.GetTasks(11 * 60);

                Assert.True(result.Result is OkObjectResult);

                resultList = ((TaskItem[])((OkObjectResult)result.Result).Value);

                Assert.Single(resultList);
                Assert.False(resultList[0].IsRunning);

                Assert.Equal(6, resultList[0].TimeLogCount);

                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpent);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentThisWeek);
                Assert.Equal(12 * HOURS, resultList[0].TotalTimeSpentToday);

                // UTC-7
                result = controller.GetTasks(-7 * 60);

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
            var timeProvider = TestUtils.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(new DateTime(2020, 8, 18, 16, 35, 0), 1 * HOURS, false)
                    .CreateTimeLog(new DateTime(2020, 8, 17, 22, 59, 0), 1 * HOURS, false)
                    .CreateTimeLog(new DateTime(2020, 8, 17, 21, 31, 0), 1 * HOURS, false)
;

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                // UTC
                var result = controller.GetTasks(-420);

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
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, TestUtils.CreateTimerProvider());

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
        public void GetTimelogAsyncReturnAllTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddDays(-12f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-8f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-6f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-4f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, null, null, null);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(6, resultList.Items.Length);
                Assert.Equal(6, resultList.TotalCount);
                Assert.Equal(6, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(time.AddDays(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTodayTest()
        {
            var time = new DateTime(2020, 8, 12, 15, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
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
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, null, null, TimeLogResultType.Day);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(3, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(3, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(time.AddHours(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnWeekTest()
        {
            var time = new DateTime(2020, 8, 12, 15, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
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
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, null, null, TimeLogResultType.Week);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(5, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(5, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(time.AddHours(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTop5Test()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
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
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, 5, null, TimeLogResultType.All);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(5, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(9, resultList.Count);
                Assert.Equal(0, resultList.Page);
                Assert.Equal(time.AddHours(-2f), resultList.Items[0].StartTime);
            }
        }

        [Fact()]
        public void GetTimelogAsyncReturnTop5Page2Test()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
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
                    .CreateTimeLog(time.AddDays(-2f), 2 * HOURS, false);

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager, timeProvider);

                var result = controller.GetTimelogAsync(db_test.lastTask.PublicId, 0, 5, 1, TimeLogResultType.All);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ModelList<TimeLogItem>)((OkObjectResult)result.Result).Value);
                Assert.Equal(4, resultList.Items.Length);
                Assert.Equal(9, resultList.TotalCount);
                Assert.Equal(9, resultList.Count);
                Assert.Equal(1, resultList.Page);
                Assert.Equal(time.AddDays(-4f), resultList.Items[0].StartTime);
            }
        }
    }
}