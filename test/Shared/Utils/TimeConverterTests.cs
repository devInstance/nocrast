using Xunit;
using NoCrast.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoCrast.Shared.Utils.Tests
{
    public class TimeConverterTests
    {
        [Fact()]
        public void GetStartOfTheDayForTimeOffsetUTCMinusSevenTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var expectedTime = time.AddDays(-1).AddHours(7);
            var result = TimeConverter.GetStartOfTheDayForTimeOffset(time, -7 * 60);
            Assert.Equal(expectedTime, result);
        }

        [Fact()]
        public void GetStartOfTheDayForTimeOffsetUTCTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var expectedTime = new DateTime(2020, 8, 10, 0, 0, 0);
            var result = TimeConverter.GetStartOfTheDayForTimeOffset(time, 0);
            Assert.Equal(expectedTime, result);
        }

        [Fact()]
        public void GetStartOfTheWeekForTimeOffsetUTCMinusSevenTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var expectedTime = time.AddDays(-7).AddHours(7);
            var result = TimeConverter.GetStartOfTheWeekForTimeOffset(time, -7 * 60);
            Assert.Equal(expectedTime, result);
        }

        [Fact()]
        public void GetStartOfTheWeekForTimeOffsetUTCTest()
        {
            var time = new DateTime(2020, 8, 10, 0, 0, 0);
            var expectedTime = new DateTime(2020, 8, 10, 0, 0, 0);
            var result = TimeConverter.GetStartOfTheWeekForTimeOffset(time, 0);
            Assert.Equal(expectedTime, result);
        }
    }
}