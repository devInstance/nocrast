using Xunit;
using NoCrast.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Shared.Model;

namespace NoCrast.Server.Controllers.Tests
{
    public class TasksControllerTests
    {
        [Fact()]
        public void GetTasksTest()
        {
            using (TestDatabase db_test = new TestDatabase())
            {
                db_test.UserProfile().CreateTask("Task 1").CreateTask("Task 2");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager);

                var result = controller.GetTasks();

                Assert.True(result.Result is OkObjectResult);
                Assert.Equal(2, ((TaskItem[])((OkObjectResult)result.Result).Value).Length);
            }
        }

        [Fact()]
        public void AddTaskTest()
        {
            using (TestDatabase db_test = new TestDatabase())
            {
                db_test.UserProfile();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TasksController(db_test.db, userManager);

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
    }
}