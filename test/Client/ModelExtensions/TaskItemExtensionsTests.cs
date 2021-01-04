using Moq;
using NoCrast.Shared.Utils;
using System;
using Xunit;

namespace NoCrast.Client.ModelExtensions.Tests
{
    public class TaskItemExtensionsTests
    {
        private static Mock<ITimeProvider> SetupTimerProvider(DateTime date)
        {
            Mock<ITimeProvider> timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(t => t.CurrentTime).Returns(date);
            return timeProvider;
        }

        //[Fact]
        //public void TaskTest()
        //{
        //    var t = new TaskItem { Title = "test" };

        //    Mock<ITimeProvider> timeProvider = SetupTimerProvider(new DateTime(2020, 4, 10, 7, 0, 0));
        //    Assert.Equal("test", t.Title);
        //    Assert.False(t.IsRunning);
        //    Assert.Equal(0, t.GetElapsedTimeSpan(timeProvider.Object).TotalMilliseconds);
        //}

        //[Fact]
        //public void StartTest()
        //{
        //    var t = new TaskItem { Title = "test" };
        //    var date = new DateTime(2020, 4, 10, 7, 0, 0);
        //    Mock<ITimeProvider> timeProvider = SetupTimerProvider(date);

        //    t.Start(timeProvider.Object);

        //    date = date.AddSeconds(3);
        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);
        //    Assert.True(t.IsRunning);
        //    Assert.True(t.GetElapsedTimeSpan(timeProvider.Object).TotalSeconds == 3.0f);
        //}

        //[Fact]
        //public void StopTest()
        //{
        //    var t = new TaskItem { Title = "test" };
        //    var date = new DateTime(2020, 4, 10, 7, 0, 0);
        //    Mock<ITimeProvider> timeProvider = SetupTimerProvider(date);

        //    t.Start(timeProvider.Object);

        //    date = date.AddSeconds(3);
        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);

        //    t.Stop(timeProvider.Object);

        //    Assert.False(t.IsRunning);

        //    date = date.AddSeconds(3);
        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);

        //    Assert.True(t.GetElapsedTimeSpan(timeProvider.Object).TotalSeconds == 3.0f);
        //}

        //[Fact]
        //public void MultipleStopTest()
        //{
        //    var t = new TaskItem { Title = "test" };
        //    var date = new DateTime(2020, 4, 10, 7, 0, 0);
        //    Mock<ITimeProvider> timeProvider = SetupTimerProvider(date);

        //    t.Start(timeProvider.Object);

        //    date = date.AddSeconds(3);
        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);

        //    t.Stop(timeProvider.Object);

        //    date = date.AddSeconds(2);

        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);
        //    t.Start(timeProvider.Object);

        //    date = date.AddSeconds(3);
        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);

        //    t.Stop(timeProvider.Object);

        //    date = date.AddSeconds(3);
        //    timeProvider.Setup(t => t.CurrentTime).Returns(date);

        //    Assert.False(t.IsRunning);
        //    Assert.True(t.GetElapsedTimeSpan(timeProvider.Object).TotalSeconds == 6.0f);
        //}
    }
}