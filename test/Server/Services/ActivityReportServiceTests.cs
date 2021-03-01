using Xunit;
using NoCrast.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoCrast.ServerTests;
using NoCrast.Server.Queries;
using Moq;
using NoCrast.Server.Model;

namespace NoCrast.Server.Services.Tests
{
    public class ActivityReportServiceTests
    {
        private static void AddTestCase(Mock<IActivityReportSelect> mockSelect, long start, long duration,  long returnValue)
        {
            mockSelect.Setup(x => x.GetTotalForPeriod(It.IsAny<UserProfile>(), It.Is<long>(a => a.Equals(start)), It.Is<long>(a => a.Equals(start + duration)))).Returns(returnValue);
        }

        [Fact()]
        public void GetActivityReportTest()
        {
            var time = new DateTime(2016, 7, 12, 14, 0, 0);
            var timeProvider = TestUtils.CreateTimerProvider(time);

            var mockSelect = new Mock<IActivityReportSelect>();
            AddTestCase(mockSelect, 15, 15, 25);

            ActivityReportService service = new ActivityReportService(timeProvider, mockSelect.Object);

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

            var mockSelect = new Mock<IActivityReportSelect>();
            mockSelect.Setup(x => x.GetTotalForPeriod(It.IsAny<UserProfile>(), It.IsAny<long>(), It.IsAny<long>())).Returns(0);

            ActivityReportService service = new ActivityReportService(timeProvider, mockSelect.Object);

            var result = service.GetActivityReport(null, interval, startOfDay, columnsCount);

            Assert.Equal(columnsCount, result.Rows[0].Data.Length);
            Assert.Equal(columnsCount, result.Columns.Length);

            Assert.Equal(startOfDay, result.Columns[0]);
            Assert.Equal(startOfDay.AddMinutes(interval), result.Columns[1]);
            Assert.Equal(startOfDay.AddMinutes(interval * (columnsCount - 1)), result.Columns[columnsCount - 1]);

        }

    }
}