﻿using Xunit;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Shared.Model;
using NoCrast.TestUtils;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class TagsControllerTests
    {
        [Fact()]
        public void GetEmptyTagsTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test A").CreateTag("Test B").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var result = controller.GetTags(false);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TagItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Name);
                Assert.Equal(0, resultList[0].TotalTimeSpent);
                Assert.Equal(0, resultList[0].TasksCount);
                Assert.Equal("Test B", resultList[1].Name);
                Assert.Equal(0, resultList[1].TasksCount);
                Assert.Equal(0, resultList[1].TotalTimeSpent);
            }
        }

        [Fact()]
        public void GetTagsWithOutTotalsTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                        .CreateTimeLog(TimerProviderMock.TEST_TIME.AddHours(-4), 2 * 60 * 1000, false)
                        .CreateTimeLog(TimerProviderMock.TEST_TIME, 2 * 60 * 1000, false)
                        .CreateTag("Test A").AssignLastTag()
                        .CreateTag("Test B").AssignLastTag()
                        .CreateTask("Task 2").AssignLastTag()
                        .CreateTimeLog(TimerProviderMock.TEST_TIME, 2 * 60 * 1000, false)
                        .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var result = controller.GetTags(false);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TagItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Name);
                Assert.Equal(0, resultList[0].TasksCount);
                Assert.Equal(0, resultList[0].TotalTimeSpent);
                Assert.Equal("Test B", resultList[1].Name);
                Assert.Equal(0, resultList[1].TasksCount);
                Assert.Equal(0, resultList[1].TotalTimeSpent);
            }
        }

        [Fact()]
        public void GetTagsWithTotalsTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                        .CreateTimeLog(TimerProviderMock.TEST_TIME.AddHours(-4), 2 * 60 * 1000, false)
                        .CreateTimeLog(TimerProviderMock.TEST_TIME, 2 * 60 * 1000, false)
                        .CreateTag("Test A").AssignLastTag()
                        .CreateTag("Test B").AssignLastTag()
                        .CreateTask("Task 2").AssignLastTag()
                        .CreateTimeLog(TimerProviderMock.TEST_TIME, 2 * 60 * 1000, false)
                        .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var result = controller.GetTags(true);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TagItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Name);
                Assert.Equal(1, resultList[0].TasksCount);
                Assert.Equal(4 * 60 * 1000, resultList[0].TotalTimeSpent);
                Assert.Equal("Test B", resultList[1].Name);
                Assert.Equal(2, resultList[1].TasksCount);
                Assert.Equal(6 * 60 * 1000, resultList[1].TotalTimeSpent);
            }
        }

        [Fact()]
        public void GetTagsMultiUsersTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test A").CreateTag("Test B");
                db_test.UserProfile().EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var result = controller.GetTags(false);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TagItem[])((OkObjectResult)result.Result).Value);
                Assert.Empty(resultList);
            }
        }

        [Fact()]
        public void GetTagsTaskCountTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1").CreateTag("Test A").AssignLastTag().CreateTag("Test B").AssignLastTag()
                    .CreateTask("Task 2").AssignLastTag().CreateTask("Test 3").AssignLastTag().EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var result = controller.GetTags(true);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TagItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Name);
                Assert.Equal(1, resultList[0].TasksCount);
                Assert.Equal(0, resultList[0].TotalTimeSpent);
                Assert.Equal("Test B", resultList[1].Name);
                Assert.Equal(3, resultList[1].TasksCount);
                Assert.Equal(0, resultList[1].TotalTimeSpent);
            }
        }

        [Fact()]
        public void AddTagTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test B").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var newTag = new TagItem
                {
                    Name = "Test A"
                };
                
                var result = controller.AddTag(newTag);
                Assert.True(result.Result is OkObjectResult);
                var resultValue = ((TagItem)((OkObjectResult)result.Result).Value);
                Assert.Equal("Test A", resultValue.Name);
                Assert.NotEmpty(resultValue.Id);
            }
        }

        [Fact()]
        public void UpdateTagTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test B").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var newTag = new TagItem
                {
                    Id = db_test.lastTag.PublicId,
                    Name = "Test A"
                };

                var result = controller.UpdateTag(newTag.Id, newTag);
                Assert.True(result.Result is OkObjectResult);
                var resultValue = ((TagItem)((OkObjectResult)result.Result).Value);
                Assert.Equal(newTag.Id, resultValue.Id);
                Assert.Equal("Test A", resultValue.Name);
                Assert.NotEmpty(resultValue.Id);
            }
        }

        [Fact()]
        public void RemoveTagTest()
        {
            var timeProvider = TimerProviderMock.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test B").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var newTag = new TagItem
                {
                    Id = db_test.lastTag.PublicId,
                    Name = "Test A"
                };

                var result = controller.RemoveTag(newTag.Id);
                Assert.True(result.Result is OkObjectResult);
                var resultValue = ((bool)((OkObjectResult)result.Result).Value);
                Assert.True(resultValue);
            }
        }
    }
}