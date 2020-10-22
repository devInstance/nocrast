using Xunit;
using NoCrast.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Server.Model;
using NoCrast.Shared.Model;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class UserProfileControllerTests
    {
        [Fact()]
        public void GetProfileTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("Test").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new UserProfileController(db_test.db, userManager, timeProvider);

                var result = controller.GetProfile();

                Assert.True(result.Result is OkObjectResult);
                var resultObj = ((UserProfileItem)((OkObjectResult)result.Result).Value);
                Assert.Equal("Test", resultObj.Name);
                Assert.Equal("Test", resultObj.Email);
            }
        }

        [Fact()]
        public void UpdateProfileTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile("Test").EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new UserProfileController(db_test.db, userManager, timeProvider);

                var newUser = new UserProfileItem
                {
                    Email = db_test.profile.Email,
                    Name = "Test 2"
                };

                var result = controller.UpdateProfile(newUser);

                Assert.True(result.Result is OkObjectResult);
                var resultObj = ((UserProfileItem)((OkObjectResult)result.Result).Value);
                Assert.Equal("Test 2", resultObj.Name);
                Assert.Equal("Test", resultObj.Email);
            }
        }
    }
}