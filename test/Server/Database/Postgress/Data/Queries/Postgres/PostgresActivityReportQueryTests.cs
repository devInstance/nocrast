using Xunit;
using System;
using NoCrast.Server.PostgressTests;
using NoCrast.Shared.Utils;
using Moq;
using NoCrast.TestUtils;
using NoCrast.Server.Database.Postgres.Data.Queries;

namespace NoCrast.Server.Data.Queries.Postgres.Tests
{
    public class PostgresActivityReportQueryTests
    {
        static long MINUTES = 60 * 1000;
        static long HOURS = 60 * MINUTES;
        static DateTime STARTDATE = new DateTime(2020, 8, 12, 14, 0, 0);

        [Theory()]
        [InlineData(13 * 60,        13 * 60 + 15,   0, "Task 1", null, null)]
        [InlineData(13 * 60 + 45,   14 * 60,        10, "Task 1", null, null)]
        [InlineData(14 * 60,        14 * 60 + 15,   25, "Task 1", null, null)]
        [InlineData(14 * 60 + 15,   14 * 60 + 30,   40, "Task 1", null, null)]
        [InlineData(14 * 60 + 30,   14 * 60 + 45,   35, "Task 1", null, null)]
        [InlineData(14 * 60 + 45,   15 * 60     ,   45, "Task 1", null, null)]
        [InlineData(15 * 60,        15 * 60 + 15,   5, "Task 1", null, null)]

        [InlineData(13 * 60,        13 * 60 + 15,   0 * 2, null, null, null)]
        [InlineData(13 * 60 + 45,   14 * 60,        10 * 2, null, null, null)]
        [InlineData(14 * 60,        14 * 60 + 15,   25 * 2, null, null, null)]
        [InlineData(14 * 60 + 15,   14 * 60 + 30,   40 * 2, null, null, null)]
        [InlineData(14 * 60 + 30,   14 * 60 + 45,   35 * 2, null, null, null)]
        [InlineData(14 * 60 + 45,   15 * 60,        45 * 2, null, null, null)]
        [InlineData(15 * 60,        15 * 60 + 15,   5 * 2, null, null, null)]

        [InlineData(13 * 60,        13 * 60 + 15,   0 * 0, "Task 1", "2020-8-12 10:00:00", "2020-8-12 12:00:00")]
        [InlineData(14 * 60,        14 * 60 + 15,   25 * 2, null, "2019-8-12 10:00:00", "2021-8-12 12:00:00")]
        [InlineData(14 * 60 + 45,   15 * 60,        15 * 2, null, "2019-8-12 10:00:00", "2020-4-13 12:00:00")]
        [InlineData(14 * 60,        14 * 60 + 15,   10 * 1, "Task 1", "2020-8-12 00:00:00", "2020-8-13 00:00:00")]
        [InlineData(14 * 60,        14 * 60 + 15,   10 * 2, null, "2020-8-12 00:00:00", "2020-8-13 00:00:00")]
        [InlineData(14 * 60,        14 * 60 + 15,   0, "Task 1", "2020-9-12 00:00:00", "2020-9-13 12:00:00")]
        public void PeriodSumTest(long start, long stop, long expectedResult, string taskid, string begin, string end)
        {
            var time = STARTDATE;

            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.CurrentTime).Returns(time);

            using (TestDatabase db_test = new TestDatabase(timeProvider.Object))
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
                
                time = STARTDATE;
                db_test.CreateTask("Task 2")
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

                var query = new PostgresActivityReportQuery(new IScopeManagerMock(), timeProvider.Object, db_test.db, db_test.profile);
                if (taskid != null)
                {
                    query.Task(taskid);
                }
                if(begin  != null)
                {
                    query.Start(DateTime.Parse(begin));
                }
                if (end != null)
                {
                    query.End(DateTime.Parse(end));
                }

                var result = query.PeriodSum(start, stop);

                Assert.Equal(expectedResult, result);
            }
        }
    }
}