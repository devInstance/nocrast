using Xunit;
using System;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Shared.Model;
using static NoCrast.Shared.Model.ReportItem;
using NoCrast.TestUtils;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class ReportControllerTests
    {
        static long MINUTES = 60 * 1000;
        static long HOURS = 60 * MINUTES;

        [Fact()]
        public void GetActivityReportTest()
        {
            var time = new DateTime(2016, 7, 12, 14, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddMinutes(-5), 15 * MINUTES, false);
                time = time.AddDays(1);
                db_test.CreateTimeLog(time.AddMinutes(-5), 70 * MINUTES, false);
                time = time.AddMonths(-1);
                db_test.CreateTimeLog(time.AddMinutes(15), 15 * MINUTES, false);
                time = time.AddMonths(-2);
                db_test.CreateTimeLog(time.AddMinutes(45), 15 * MINUTES, false);
                time = time.AddMonths(-5);
                db_test.CreateTimeLog(time.AddMinutes(30), 30 * MINUTES, false);
                time = time.AddMonths(-5);
                db_test.CreateTimeLog(time.AddMinutes(20), 15 * MINUTES, false);

                db_test.EndSetup();


                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new ReportController(db_test.db, userManager, null, null); //TODO: Fix me

                var result = controller.GetActivityReport(0);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ReportItem)((OkObjectResult)result.Result).Value);

                Assert.Equal(96, resultList.Columns.Length);
                Assert.Single(resultList.Rows);

                Assert.Equal(96, resultList.Rows[0].Data.Length);
                Assert.Equal(25.0f/45.0f, resultList.Rows[0].Data[56].Value);
                Assert.Equal(40.0f/45.0f, resultList.Rows[0].Data[57].Value);
                Assert.Equal(35.0f/45.0f, resultList.Rows[0].Data[58].Value);
                Assert.Equal(45.0f/45.0f, resultList.Rows[0].Data[59].Value);
            }
        }
    }
}