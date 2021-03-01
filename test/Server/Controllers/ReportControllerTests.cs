using Xunit;
using System;
using NoCrast.ServerTests;
using Microsoft.AspNetCore.Mvc;
using NoCrast.Shared.Model;
using static NoCrast.Shared.Model.ReportItem;

namespace NoCrast.Server.Controllers.Tests
{
    [Collection("DBTests")]
    public class ReportControllerTests
    {
        static long MINUTES = 60 * 1000;
        static long HOURS = 60 * MINUTES;

        [Fact()]
        public void GetDailyReportTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddDays(10f), 2 * HOURS, false);

                time = time.AddDays(1);
                db_test.CreateTask("Task 2")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new ReportController(db_test.db, userManager, timeProvider, null);

                var result = controller.GetDailyReport(0, timeProvider.CurrentTime);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ReportItem)((OkObjectResult)result.Result).Value);

                Assert.Equal(RIType.Daily, resultList.RiType);
                Assert.Equal(7, resultList.Columns.Length);
                Assert.Equal(2, resultList.Rows.Length);

                Assert.Equal("Task 1", resultList.Rows[0].TaskTitle);
                Assert.Equal(8, resultList.Rows[0].Data.Length);
                Assert.Equal(0, resultList.Rows[0].Data[0]);
                Assert.Equal(4 * HOURS, resultList.Rows[0].Data[1]);
                Assert.Equal(8 * HOURS, resultList.Rows[0].Data[2]);
                Assert.Equal(0, resultList.Rows[0].Data[3]);
                Assert.Equal(0, resultList.Rows[0].Data[5]);
                Assert.Equal(12 * HOURS, resultList.Rows[0].Data[7]);

                Assert.Equal("Task 2", resultList.Rows[1].TaskTitle);
                Assert.Equal(8, resultList.Rows[1].Data.Length);
                Assert.Equal(0, resultList.Rows[1].Data[0]);
                Assert.Equal(4 * HOURS, resultList.Rows[1].Data[2]);
                Assert.Equal(8 * HOURS, resultList.Rows[1].Data[3]);
                Assert.Equal(0, resultList.Rows[1].Data[4]);
                Assert.Equal(0, resultList.Rows[1].Data[5]);
                Assert.Equal(12 * HOURS, resultList.Rows[1].Data[7]);

                Assert.Equal(new DateTime(2020, 8, 10), resultList.StartDate.Date);
                Assert.Equal(new DateTime(2020, 8, 16), resultList.EndDate.Date);

            }
        }

        [Fact()]
        public void GetWeeklyReportTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddDays(10f), 2 * HOURS, false);

                time = time.AddDays(1);
                db_test.CreateTask("Task 2")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)
                    .EndSetup();

                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new ReportController(db_test.db, userManager, timeProvider, null);

                var result = controller.GetWeeklyReport(0, timeProvider.CurrentTime);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ReportItem)((OkObjectResult)result.Result).Value);

                Assert.Equal(RIType.Weekly, resultList.RiType);
                Assert.Equal(5, resultList.Columns.Length);
                Assert.Equal(2, resultList.Rows.Length);

                Assert.Equal("Task 1", resultList.Rows[0].TaskTitle);
                Assert.Equal(6, resultList.Rows[0].Data.Length);
                Assert.Equal(0, resultList.Rows[0].Data[0]);
                Assert.Equal(0, resultList.Rows[0].Data[1]);
                Assert.Equal(2 * HOURS, resultList.Rows[0].Data[2]);
                Assert.Equal(0, resultList.Rows[0].Data[3]);
                Assert.Equal(12 * HOURS, resultList.Rows[0].Data[4]);
                Assert.Equal(14 * HOURS, resultList.Rows[0].Data[5]);

                Assert.Equal("Task 2", resultList.Rows[1].TaskTitle);
                Assert.Equal(6, resultList.Rows[1].Data.Length);
                Assert.Equal(0, resultList.Rows[1].Data[0]);
                Assert.Equal(0, resultList.Rows[1].Data[1]);
                Assert.Equal(0, resultList.Rows[1].Data[2]);
                Assert.Equal(12 * HOURS, resultList.Rows[1].Data[4]);
                Assert.Equal(12 * HOURS, resultList.Rows[1].Data[5]);

                Assert.Equal(new DateTime(2020, 7, 13), resultList.StartDate.Date);
                Assert.Equal(new DateTime(2020, 8, 16), resultList.EndDate.Date);

            }
        }

        [Fact()]
        public void GetMonthlyReportTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
            using (TestDatabase db_test = new TestDatabase(timeProvider))
            {
                db_test.UserProfile().CreateTask("Task 1")
                    .CreateTimeLog(time.AddMonths(-1), 2 * HOURS, false)
                    .CreateTimeLog(time.AddDays(-10f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddDays(10f), 2 * HOURS, false)

                    .CreateTimeLog(time.AddMonths(2), 2 * HOURS, false);

                time = time.AddDays(1);
                db_test.CreateTask("Task 2")
                    .CreateTimeLog(time.AddHours(-10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(-5f), 2 * HOURS, false)
                    .CreateTimeLog(time, 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(5f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(10f), 2 * HOURS, false)
                    .CreateTimeLog(time.AddHours(15f), 2 * HOURS, false)
                    .EndSetup();


                UserManagerMock userManager = new UserManagerMock(db_test.profile.ApplicationUserId);

                var controller = new ReportController(db_test.db, userManager, timeProvider, null);

                var result = controller.GetMonthlyReport(0, timeProvider.CurrentTime);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ReportItem)((OkObjectResult)result.Result).Value);

                Assert.Equal(RIType.Monthly, resultList.RiType);
                Assert.Equal(12, resultList.Columns.Length);
                Assert.Equal(2, resultList.Rows.Length);

                Assert.Equal("Task 1", resultList.Rows[0].TaskTitle);
                Assert.Equal(13, resultList.Rows[0].Data.Length);
                Assert.Equal(0, resultList.Rows[0].Data[0]);
                Assert.Equal(0, resultList.Rows[0].Data[1]);
                Assert.Equal(0, resultList.Rows[0].Data[2]);
                Assert.Equal(2 * HOURS, resultList.Rows[0].Data[6]);
                Assert.Equal(16 * HOURS, resultList.Rows[0].Data[7]);
                Assert.Equal(0, resultList.Rows[0].Data[8]);
                Assert.Equal(2 * HOURS, resultList.Rows[0].Data[9]);
                Assert.Equal(20 * HOURS, resultList.Rows[0].Data[12]);

                Assert.Equal("Task 2", resultList.Rows[1].TaskTitle);
                Assert.Equal(13, resultList.Rows[1].Data.Length);
                Assert.Equal(0, resultList.Rows[1].Data[0]);
                Assert.Equal(0, resultList.Rows[1].Data[1]);
                Assert.Equal(0, resultList.Rows[1].Data[2]);
                Assert.Equal(12 * HOURS, resultList.Rows[1].Data[7]);
                Assert.Equal(0, resultList.Rows[1].Data[8]);
                Assert.Equal(12 * HOURS, resultList.Rows[1].Data[12]);

                Assert.Equal(new DateTime(2020, 1, 1), resultList.StartDate.Date);
                Assert.Equal(new DateTime(2020, 12, 31), resultList.EndDate.Date);

            }
        }
        [Fact()]
        public void GetActivityReportTest()
        {
            var time = new DateTime(2016, 7, 12, 14, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);
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

                var controller = new ReportController(db_test.db, userManager, timeProvider, null);

                var result = controller.GetActivityReport(0);

                Assert.True(result.Result is OkObjectResult);
                var resultList = ((ReportItem)((OkObjectResult)result.Result).Value);

                Assert.Equal(96, resultList.Columns.Length);
                Assert.Single(resultList.Rows);

                Assert.Equal(96, resultList.Rows[0].Data.Length);
                Assert.Equal(25.0f/45.0f, resultList.Rows[0].Data[56]);
                Assert.Equal(40.0f/45.0f, resultList.Rows[0].Data[57]);
                Assert.Equal(35.0f/45.0f, resultList.Rows[0].Data[58]);
                Assert.Equal(45.0f/45.0f, resultList.Rows[0].Data[59]);
            }
        }
    }
}