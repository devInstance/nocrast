using Xunit;
using System;
using NoCrast.Server.PostgressTests;
using NoCrast.Shared.Utils;
using Moq;
using NoCrast.TestUtils;

namespace NoCrast.Server.Data.Queries.Postgres.Tests
{
    public class PostgresAggregateReportQueryTests
    {
        static long MINUTES = 60 * 1000;
        static long HOURS = 60 * MINUTES;
        static DateTime STARTDATE = new DateTime(2020, 8, 12, 0, 0, 0);

        [Theory()]
        [InlineData("2020-8-12 10:00:00", "2020-8-12 12:00:00", 2, false)]
        [InlineData("2020-8-13 10:00:00", "2020-8-13 12:00:00", 2, false)]
        [InlineData("2020-8-12 10:00:00", "2020-8-12 12:00:00", 0, true)]
//        [InlineData("2020-8-13 10:00:00", "2020-8-13 11:00:00", 1, true)] //TODO: we have to support this case eventually
        [InlineData("2020-8-13 00:00:00", "2020-8-13 23:00:00", 8, true)]
        [InlineData("2020-8-12 00:00:00", "2020-8-13 23:00:00", 12, true)]
        [InlineData("2020-8-12 00:00:00", "2020-8-13 23:00:00", 20, false)]
        public void PeriodSumTest(string startDate, string endDate, long expectedResultInHours, bool filterLastTask)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var time = STARTDATE;

            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.CurrentTime).Returns(time);

            using (TestDatabase db_test = new TestDatabase(timeProvider.Object))
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

                var query = new PostgresAggregateReportQuery(new IScopeManagerMock(), timeProvider.Object, db_test.db, db_test.profile);

                if (filterLastTask)
                {
                    query.Task(db_test.lastTask);
                }
                var result = query.PeriodSum(start, end);

                Assert.Equal(expectedResultInHours * 1000 * 60 * 60, result);
            }
        }
    }
}