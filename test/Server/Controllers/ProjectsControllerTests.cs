using Xunit;
using NoCrast.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using NoCrast.ServerTests;
using NoCrast.Shared.Model;
using Microsoft.AspNetCore.Mvc;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class ProjectsControllerTests
    {
        [Fact()]
        public void GetProjectsTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("Test A").CreateProject("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new ProjectsController(db_test.db, userManager, timeProvider);

                var result = controller.GetProjects(false);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ProjectItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Title);
                Assert.Null(resultList[0].Descritpion);
                Assert.Equal(0, resultList[0].TasksCount);
                Assert.Equal(0, resultList[0].TotalTimeSpent);
                Assert.Equal("Test B", resultList[1].Title);
            }
        }

        [Fact()]
        public void GetProjectsWithZeroTotalsTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("Test A").CreateProject("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new ProjectsController(db_test.db, userManager, timeProvider);

                var result = controller.GetProjects(true);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ProjectItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Title);
                Assert.Null(resultList[0].Descritpion);
                Assert.Equal(0, resultList[0].TasksCount);
                Assert.Equal(0, resultList[0].TotalTimeSpent);
                Assert.Equal("Test B", resultList[1].Title);
                Assert.Null(resultList[1].Descritpion);
                Assert.Equal(0, resultList[1].TasksCount);
                Assert.Equal(0, resultList[1].TotalTimeSpent);
            }
        }

        [Fact()]
        public void GetProjectsWithTotalsTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile()
                    .CreateProject("Test A")
                        .CreateTask("Task 1")
                            .CreateTimeLog(TestUtils.TEST_TIME.AddHours(-4), 2 * 60 * 1000, false)
                            .CreateTimeLog(TestUtils.TEST_TIME, 2 * 60 * 1000, false)
                        .CreateTask("Test 2")
                            .CreateTimeLog(TestUtils.TEST_TIME, 2 * 60 * 1000, false)
                    .CreateProject("Test B")
                        .CreateTask("Test 3")
                            .CreateTimeLog(TestUtils.TEST_TIME, 2 * 60 * 1000, false)
                    .CreateProject("Test C");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new ProjectsController(db_test.db, userManager, timeProvider);

                var result = controller.GetProjects(true);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ProjectItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(3, resultList.Length);
                Assert.Equal("Test A", resultList[0].Title);
                Assert.Equal(2, resultList[0].TasksCount);
                Assert.Equal(6 * 60 * 1000, resultList[0].TotalTimeSpent);
                Assert.Equal("Test B", resultList[1].Title);
                Assert.Equal(1, resultList[1].TasksCount);
                Assert.Equal(2 * 60 * 1000, resultList[1].TotalTimeSpent);
                Assert.Equal("Test C", resultList[2].Title);
                Assert.Equal(0, resultList[2].TasksCount);
                Assert.Equal(0, resultList[2].TotalTimeSpent);
            }
        }

        [Fact()]
        public void AddProjectTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new ProjectsController(db_test.db, userManager, timeProvider);

                var newProject = new ProjectItem
                {
                    Title = "Test A",
                    Descritpion = "test test"
                };

                var result = controller.AddProject(newProject);
                Assert.True(result.Result is OkObjectResult);
                var resultValue = ((ProjectItem)((OkObjectResult)result.Result).Value);
                Assert.Equal("Test A", resultValue.Title);
                Assert.Equal("test test", resultValue.Descritpion);
                //TODO: Check create and update dates
                Assert.NotEmpty(resultValue.Id);
            }
        }

        [Fact()]
        public void UpdateProjectTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new ProjectsController(db_test.db, userManager, timeProvider);

                var newProject = new ProjectItem
                {
                    Id = db_test.lastProject.PublicId,
                    Title = "Test A",
                    Descritpion = "test test"
                };

                var result = controller.UpdateProject(newProject.Id, newProject);
                Assert.True(result.Result is OkObjectResult);
                var resultValue = ((ProjectItem)((OkObjectResult)result.Result).Value);
                Assert.Equal(newProject.Id, resultValue.Id);
                Assert.Equal("Test A", resultValue.Title);
                Assert.Equal("test test", resultValue.Descritpion);
                //TODO: Check update date
                Assert.NotEmpty(resultValue.Id);
            }
        }

        [Fact()]
        public void RemoveProjectTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new ProjectsController(db_test.db, userManager, timeProvider);

                var newProject = new ProjectItem
                {
                    Id = db_test.lastProject.PublicId,
                    Title = "Test A"
                };

                var result = controller.RemoveProject(newProject.Id);

                ///TODO: Add function to TestDatabase ti check whether table is empty
                Assert.True(result.Result is OkObjectResult);
                var resultValue = ((bool)((OkObjectResult)result.Result).Value);
                Assert.True(resultValue);
            }
        }
    }
}