using Xunit;
using NoCrast.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using NoCrast.ServerTests;
using Moq;
using Microsoft.AspNetCore.Identity;
using NoCrast.Server.Model;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Indentity;
using System.Threading.Tasks;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class AuthorizationControllerTests
    {
        [Fact()]
        public void DeleteUsedAccountTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateProject("Proj1")
                    .CreateTask("Task 1")
                    .CreateTask("Task 2")
                    .CreateTag("Tag 1").AssignLastTag()
                    .CreateTimeLog(timeProvider.CurrentTime, 10000, true)
                    .EndSetup();

                var userManager = new UserManagerMock(db_test.profile.ApplicationUserId);
                var signinManagerMq = TestUtils.CreateSignManager(true);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.Delete();

                Assert.True(result.Result is OkObjectResult);

                Assert.Null(controller.CurrentProfile);

                signinManagerMq.Verify(x => x.SignOutAsync(), Times.Once());
                signinManagerMq.Verify();

                var user = db_test.FetchUser("nobody");
                Assert.Null(user);
                var project = db_test.FetchTag("Proj1");
                Assert.Null(project);
                var taskl = db_test.FetchTask("Task 1");
                Assert.Null(taskl);
                var taskc = db_test.FetchTask("Task 2");
                Assert.Null(taskc);
                var tag = db_test.FetchTag("Tag 1");
                Assert.Null(tag);
            }
        }

        [Fact()]
        public void DeleteNewAccountTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().EndSetup();

                var userManager = new UserManagerMock(db_test.profile.ApplicationUserId);
                var signinManagerMq = TestUtils.CreateSignManager(true);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.Delete();

                Assert.True(result.Result is OkObjectResult);
                Assert.Null(controller.CurrentProfile);

                signinManagerMq.Verify(x => x.SignOutAsync(), Times.Once());
                signinManagerMq.Verify();

                var user = db_test.FetchUser("nobody");
                Assert.Null(user);
            }
        }

        [Fact()]
        public void LoginInvalidAccountTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("nobody").CreateProject("Proj1")
                    .CreateTask("Task 1")
                    .CreateTask("Task 2")
                    .CreateTag("Tag 1").AssignLastTag()
                    .CreateTimeLog(timeProvider.CurrentTime, 10000, true)
                    .EndSetup();

                var userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var signinManagerMq = TestUtils.CreateSignManager(false);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.Login(
                    new Shared.Model.LoginParameters
                    {
                        UserName = "body",
                        Password = "ssss",
                        RememberMe = false
                    });

                Assert.True(result.Result is UnauthorizedObjectResult);
                //                Assert.Null(controller.CurrentProfile);
            }
        }

        [Fact()]
        public void LoginExistingAccountTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("nobody").CreateProject("Proj1")
                    .CreateTask("Task 1")
                    .CreateTask("Task 2")
                    .CreateTag("Tag 1").AssignLastTag()
                    .CreateTimeLog(timeProvider.CurrentTime, 10000, true)
                    .EndSetup();

                var userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var signinManagerMq = TestUtils.CreateSignManager(true);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.Login(
                    new Shared.Model.LoginParameters
                    {
                        UserName = "nobody",
                        Password = "ssss",
                        RememberMe = false
                    });

                Assert.True(result.Result is OkResult);
                var users = db_test.FetchAllUsers(null);
                Assert.Single(users);
                Assert.NotNull(controller.CurrentProfile);
                Assert.Equal("nobody", controller.CurrentProfile.Email);
            }
        }

        [Fact()]
        public void LoginExistingAccountMultipleAtteptsTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("nobody").CreateProject("Proj1")
                    .CreateTask("Task 1")
                    .CreateTask("Task 2")
                    .CreateTag("Tag 1").AssignLastTag()
                    .CreateTimeLog(timeProvider.CurrentTime, 10000, true)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var signinManagerMq = TestUtils.CreateSignManager(true);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.Login(
                    new Shared.Model.LoginParameters
                    {
                        UserName = "nobody",
                        Password = "ssss",
                        RememberMe = false
                    });

                Assert.True(result.Result is OkResult);

                result = controller.Login(
                    new Shared.Model.LoginParameters
                    {
                        UserName = "nobody",
                        Password = "ssss",
                        RememberMe = false
                    });

                Assert.True(result.Result is OkResult);
                var users = db_test.FetchAllUsers(null);
                Assert.Single(users);
            }
        }

        [Fact()]
        public void ChangePasswordFailedTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("nobody").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var signinManagerMq = TestUtils.CreateSignManager(true);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.ChangePassword(
                    new Shared.Model.ChangePasswordParameters
                    {
                        OldPassword = "ssss",
                        NewPassword = "dddd",
                    });

                Assert.True(result.Result is BadRequestObjectResult);

                var users = db_test.FetchAllUsers(null);
                Assert.Single(users);
            }
        }

        [Fact()]
        public void ChangePasswordTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("nobody").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId, true);

                var signinManagerMq = TestUtils.CreateSignManager(true);

                var controller = new AuthorizationController(db_test.db, userManager, signinManagerMq.Object);

                var result = controller.ChangePassword(
                    new Shared.Model.ChangePasswordParameters
                    {
                        OldPassword = "ssss",
                        NewPassword = "dddd",
                    });

                Assert.True(result.Result is OkResult);

                var users = db_test.FetchAllUsers(null);
                Assert.Single(users);
            }
        }


        /*TODO: 
         * RegisterTest
         * RegisterEmailExistsTest
         * RegisterEmailMalformTest
         * RegisterPassowrdDoesntMeetReqTest
         * LogoutTest
         * LogoutNotLogin
         * GetUserInfoTest
         */
    }
}