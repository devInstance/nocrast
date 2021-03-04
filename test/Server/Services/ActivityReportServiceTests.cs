using Xunit;
using System;
using NoCrast.ServerTests;
using Moq;
using NoCrast.Server.Model;
using DevInstance.LogScope;
using NoCrast.Server.Data.Queries;
using NoCrast.Server.Data;

namespace NoCrast.Server.Services.Tests
{
    public class ActivityReportServiceTests
    {
        private static void AddTestCase(Mock<IActivityReportQuery> mockSelect, long start, long duration,  long returnValue)
        {
            mockSelect.Setup(x => x.PeriodSum(It.Is<long>(a => a.Equals(start)), It.Is<long>(a => a.Equals(start + duration)))).Returns(returnValue);
        }

        [Fact()]
        public void GetActivityReportTest()
        {
            var time = new DateTime(2016, 7, 12, 14, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);

            var mockRepository = new Mock<IQueryRepository>();
            //It.IsAny<UserProfile>(), It.IsAny<int>(), 

            var mockSelect = new Mock<IActivityReportQuery>();
            AddTestCase(mockSelect, 15, 15, 25);

            mockRepository.Setup(x => x.GetActivityReportQuery(It.IsAny<UserProfile>())).Returns(mockSelect.Object);

            var mockLog = new Mock<IScopeManager>();

            ActivityReportService service = new ActivityReportService(new IScopeManagerMock(), timeProvider, mockRepository.Object);

            var result = service.GetActivityReport(null, 0);

            Assert.Equal(96, result.Rows[0].Data.Length);
            //Assert.Equal(25.0f / 45.0f, resultList.Rows[0].Data[56]);
            //Assert.Equal(40.0f / 45.0f, resultList.Rows[0].Data[57]);
            //Assert.Equal(35.0f / 45.0f, resultList.Rows[0].Data[58]);
            //Assert.Equal(45.0f / 45.0f, resultList.Rows[0].Data[59]);

        }

        [Theory]
        [InlineData(15, 24 * 60 / 15)]
        [InlineData(60, 6)]
        public void GetActivityReportParametersTest(int interval, int columnsCount)
        {
            var time = new DateTime(2016, 7, 12, 14, 0, 0);
            var startOfDay = time.Date;
            var timeProvider = TestUtils.CreateTimerProvider(time);

            var mockRepository = new Mock<IQueryRepository>();

            var mockSelect = new Mock<IActivityReportQuery>();
            mockSelect.Setup(x => x.PeriodSum(It.IsAny<long>(), It.IsAny<long>())).Returns(0);

            mockRepository.Setup(x => x.GetActivityReportQuery(It.IsAny<UserProfile>())).Returns(mockSelect.Object);

            ActivityReportService service = new ActivityReportService(new IScopeManagerMock(), timeProvider, mockRepository.Object);

            var result = service.GetActivityReport(null, 0, interval, startOfDay, columnsCount);

            Assert.Equal(columnsCount, result.Rows[0].Data.Length);
            Assert.Equal(columnsCount, result.Columns.Length);

            Assert.Equal(startOfDay, result.Columns[0]);
            Assert.Equal(startOfDay.AddMinutes(interval), result.Columns[1]);
            Assert.Equal(startOfDay.AddMinutes(interval * (columnsCount - 1)), result.Columns[columnsCount - 1]);

        }

    }
}