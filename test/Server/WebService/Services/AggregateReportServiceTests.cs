using Xunit;
using System;
using Moq;
using NoCrast.Server.Model;
using NoCrast.Server.Data.Queries;
using NoCrast.Server.Data;
using NoCrast.TestUtils;
using static NoCrast.Shared.Model.ReportItem;
using System.Collections.Generic;

namespace NoCrast.Server.Services.Tests
{
    public class AggregateReportServiceTests
    {
        static long MINUTES = 60 * 1000;
        static long HOURS = 60 * MINUTES;

        private static Mock<IAggregateReportQuery> AddTask(Mock<IAggregateReportQuery> mockSelect, string task)
        {
            var mockSelectReport = new Mock<IAggregateReportQuery>();
            mockSelect.Setup(x => x.Task(It.Is<string>(a => a == task))).Returns(mockSelectReport.Object);
            return mockSelectReport;
        }

        private static void AddTestCase(Mock<IAggregateReportQuery> mockSelect, DateTime start, DateTime end, long returnValue)
        {
            mockSelect.Setup(x => x.PeriodSum(It.Is<DateTime>(a => a.Equals(start)), It.Is<DateTime>(a => a.Equals(end)))).Returns(returnValue);
        }

        [Fact()]
        public void GetDailyReportTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);

            var mockRepository = new Mock<IQueryRepository>();
            var mockSelectReport = new Mock<IAggregateReportQuery>();

            var mockSelectReport1 = AddTask(mockSelectReport, "Task 1");
            mockSelectReport1.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);
            AddTestCase(mockSelectReport1, time.AddDays(-2), time.AddDays(-1), 1 * HOURS);
            AddTestCase(mockSelectReport1, time.AddDays(-1), time, 2 * HOURS);
            AddTestCase(mockSelectReport1, time, time.AddDays(1), 3 * HOURS);
            AddTestCase(mockSelectReport1, time.AddDays(1), time.AddDays(2), 2 * HOURS);
            AddTestCase(mockSelectReport1, time.AddDays(2), time.AddDays(3), 0);

            var mockSelectReport2 = AddTask(mockSelectReport, "Task 2");
            mockSelectReport2.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);
            AddTestCase(mockSelectReport2, time.AddDays(3), time.AddDays(4), 8 * HOURS);

            var mockSelectTasks = new Mock<ITasksQuery>();
            mockSelectTasks.Setup(x => x.SelectList()).Returns(new List<TimerTask>() { new TimerTask { Title = "Task 1" }, new TimerTask { Title = "Task 2" } });

            mockRepository.Setup(x => x.GetAggregateReportQuery(It.IsAny<UserProfile>())).Returns(mockSelectReport.Object);
            mockRepository.Setup(x => x.GetTasksQuery(It.IsAny<UserProfile>())).Returns(mockSelectTasks.Object);

            var service = new AggregateReportService(new IScopeManagerMock(), timeProvider, mockRepository.Object);

            var result = service.GetReport(null, RIType.Daily, 0, time);

            Assert.Equal(RIType.Daily, result.RiType);
            Assert.Equal(7, result.Columns.Length);
            Assert.Equal(2, result.Rows.Length);

            var row1 = result.Rows[0];
            Assert.Equal("Task 1", row1.Title);
            var data1 = row1.Data;
            Assert.Equal(8, data1.Length);
            Assert.Equal(1 * HOURS, data1[0].Value);
            Assert.Equal(2 * HOURS, data1[1].Value);
            Assert.Equal(3 * HOURS, data1[2].Value);
            Assert.Equal(2 * HOURS, data1[3].Value);
            Assert.Equal(0 * HOURS, data1[4].Value);
            Assert.Equal(0 * HOURS, data1[5].Value);
            Assert.Equal(0 * HOURS, data1[6].Value);
            Assert.Equal(8 * HOURS, data1[7].Value); //Total

            var row2 = result.Rows[1];
            Assert.Equal("Task 2", row2.Title);
            var data2 = row2.Data;
            Assert.Equal(8, data2.Length);
            Assert.Equal(0, data2[0].Value);
            Assert.Equal(0 * HOURS, data2[0].Value);
            Assert.Equal(0 * HOURS, data2[1].Value);
            Assert.Equal(0 * HOURS, data2[2].Value);
            Assert.Equal(0 * HOURS, data2[3].Value);
            Assert.Equal(0 * HOURS, data2[4].Value);
            Assert.Equal(8 * HOURS, data2[5].Value);
            Assert.Equal(0 * HOURS, data2[6].Value);
            Assert.Equal(8 * HOURS, data2[7].Value); //Total

            Assert.Equal(new DateTime(2020, 8, 10), result.StartDate.Date);
            Assert.Equal(new DateTime(2020, 8, 16), result.EndDate.Date);
        }

        [Fact()]
        public void GetWeeklyReportTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var startOfWeek = new DateTime(2020, 8, 10, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);

            var mockRepository = new Mock<IQueryRepository>();
            var mockSelectReport = new Mock<IAggregateReportQuery>();

            var mockSelectReport1 = AddTask(mockSelectReport, "Task 1");
            mockSelectReport1.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);
            AddTestCase(mockSelectReport1, startOfWeek.AddDays(-3 * 7), startOfWeek.AddDays(-2 * 7), 1 * HOURS);
            AddTestCase(mockSelectReport1, startOfWeek.AddDays(-2 * 7), startOfWeek.AddDays(-1 * 7), 2 * HOURS);
            AddTestCase(mockSelectReport1, startOfWeek.AddDays(-1 * 7), startOfWeek.AddDays(0 * 7), 3 * HOURS);
            AddTestCase(mockSelectReport1, startOfWeek.AddDays(0 * 7), startOfWeek.AddDays(1 * 7), 2 * HOURS);

            var mockSelectReport2 = AddTask(mockSelectReport, "Task 2");
            mockSelectReport2.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);
            AddTestCase(mockSelectReport2, startOfWeek.AddDays(-4 * 7), startOfWeek.AddDays(-3 * 7), 2 * HOURS);

            var mockSelectReport3 = AddTask(mockSelectReport, "Task 3");
            mockSelectReport3.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);

            var mockSelectTasks = new Mock<ITasksQuery>();
            mockSelectTasks.Setup(x => x.SelectList()).Returns(new List<TimerTask>() {  new TimerTask { Title = "Task 1" },
                                                                                        new TimerTask { Title = "Task 2" },
                                                                                        new TimerTask { Title = "Task 3" } 
                                                                                     });

            mockRepository.Setup(x => x.GetAggregateReportQuery(It.IsAny<UserProfile>())).Returns(mockSelectReport.Object);
            mockRepository.Setup(x => x.GetTasksQuery(It.IsAny<UserProfile>())).Returns(mockSelectTasks.Object);

            var service = new AggregateReportService(new IScopeManagerMock(), timeProvider, mockRepository.Object);

            var result = service.GetReport(null, RIType.Weekly, 0, time);

            Assert.Equal(RIType.Weekly, result.RiType);
            Assert.Equal(5, result.Columns.Length);
            Assert.Equal(3, result.Rows.Length);

            var row1 = result.Rows[0];
            Assert.Equal("Task 1", row1.Title);
            var data1 = row1.Data;
            Assert.Equal(6, data1.Length);
            Assert.Equal(0 * HOURS, data1[0].Value);
            Assert.Equal(1 * HOURS, data1[1].Value);
            Assert.Equal(2 * HOURS, data1[2].Value);
            Assert.Equal(3 * HOURS, data1[3].Value);
            Assert.Equal(2 * HOURS, data1[4].Value);
            Assert.Equal(8 * HOURS, data1[5].Value); //Total

            var row2 = result.Rows[1];
            Assert.Equal("Task 2", row2.Title);
            var data2 = row2.Data;
            Assert.Equal(6, data2.Length);
            Assert.Equal(2 * HOURS, data2[0].Value);
            Assert.Equal(0 * HOURS, data2[1].Value);
            Assert.Equal(0 * HOURS, data2[2].Value);
            Assert.Equal(0 * HOURS, data2[3].Value);
            Assert.Equal(0 * HOURS, data2[4].Value);
            Assert.Equal(2 * HOURS, data2[5].Value); //Total

            Assert.Equal(new DateTime(2020, 7, 13), result.StartDate.Date);
            Assert.Equal(new DateTime(2020, 8, 16), result.EndDate.Date);
        }

        [Fact()]
        public void GetMonthlyReportTest()
        {
            var time = new DateTime(2020, 8, 12, 0, 0, 0);
            var startOfYear = new DateTime(2020, 1, 1, 0, 0, 0);
            var timeProvider = TimerProviderMock.CreateTimerProvider(time);

            var mockRepository = new Mock<IQueryRepository>();
            var mockSelectReport = new Mock<IAggregateReportQuery>();

            var mockSelectReport1 = AddTask(mockSelectReport, "Task 1");
            mockSelectReport1.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);
            AddTestCase(mockSelectReport1, startOfYear.AddMonths(0), startOfYear.AddMonths(1), 1 * HOURS);
            AddTestCase(mockSelectReport1, startOfYear.AddMonths(2), startOfYear.AddMonths(3), 2 * HOURS);
            AddTestCase(mockSelectReport1, startOfYear.AddMonths(4), startOfYear.AddMonths(5), 3 * HOURS);

            var mockSelectReport2 = AddTask(mockSelectReport, "Task 2");
            mockSelectReport2.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);

            var mockSelectReport3 = AddTask(mockSelectReport, "Task 3");
            mockSelectReport3.Setup(x => x.PeriodSum(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(0);

            var mockSelectTasks = new Mock<ITasksQuery>();
            mockSelectTasks.Setup(x => x.SelectList()).Returns(new List<TimerTask>() {  new TimerTask { Title = "Task 1" },
                                                                                        new TimerTask { Title = "Task 2" },
                                                                                        new TimerTask { Title = "Task 3" }
                                                                                     });

            mockRepository.Setup(x => x.GetAggregateReportQuery(It.IsAny<UserProfile>())).Returns(mockSelectReport.Object);
            mockRepository.Setup(x => x.GetTasksQuery(It.IsAny<UserProfile>())).Returns(mockSelectTasks.Object);

            var service = new AggregateReportService(new IScopeManagerMock(), timeProvider, mockRepository.Object);

            var result = service.GetReport(null, RIType.Monthly, 0, time);

            Assert.Equal(RIType.Monthly, result.RiType);
            Assert.Equal(12, result.Columns.Length);
            Assert.Equal(3, result.Rows.Length);

            var row1 = result.Rows[0];
            Assert.Equal("Task 1", row1.Title);
            var data1 = row1.Data;
            Assert.Equal(13, data1.Length);
            Assert.Equal(1 * HOURS, data1[0].Value);
            Assert.Equal(0 * HOURS, data1[1].Value);
            Assert.Equal(2 * HOURS, data1[2].Value);
            Assert.Equal(0 * HOURS, data1[3].Value);
            Assert.Equal(3 * HOURS, data1[4].Value);
            Assert.Equal(0 * HOURS, data1[11].Value);
            Assert.Equal(6 * HOURS, data1[12].Value);

            var row2 = result.Rows[1];
            Assert.Equal("Task 2", row2.Title);
            var data2 = row2.Data;
            Assert.Equal(13, data2.Length);
            Assert.Equal(0 * HOURS, data2[12].Value);

            Assert.Equal(new DateTime(2020, 1, 1), result.StartDate.Date);
            Assert.Equal(new DateTime(2020, 12, 31), result.EndDate.Date);
        }

    }
}