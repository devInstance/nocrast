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
    [Collection("DBTests")]
    public class TagsControllerTests
    {
        [Fact()]
        public void GetTagsTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test A").CreateTag("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

                var controller = new TagsController(db_test.db, userManager, timeProvider);

                var result = controller.GetTags();

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((TagItem[])((OkObjectResult)result.Result).Value);
                Assert.Equal(2, resultList.Length);
                Assert.Equal("Test A", resultList[0].Name);
                Assert.Equal("Test B", resultList[1].Name);
            }
        }

        [Fact()]
        public void AddTagTest()
        {
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

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
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

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
            var timeProvider = TestUtils.CreateTimerProvider();
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTag("Test B");

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId.ToString());

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