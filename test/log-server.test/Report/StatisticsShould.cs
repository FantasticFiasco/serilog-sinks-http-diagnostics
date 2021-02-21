using System;
using LogServer.Time;
using Moq;
using Shouldly;
using Xunit;

namespace LogServer.Report
{
    public class StatisticsShould
    {
        private readonly Mock<Clock> clock;
        private readonly Statistics statistics;

        public StatisticsShould()
        {
            clock = new Mock<Clock>();
            statistics = new Statistics(clock.Object);
        }

        [Fact]
        public void InitiallyBe()
        {
            // Assert
            statistics.Start.ShouldBeNull();
            statistics.BatchCount.ShouldBe(0);
            statistics.BatchesPerMinute.ShouldBeNull();
            statistics.LogEventCount.ShouldBe(0);
            statistics.LogEventsPerMinute.ShouldBeNull();
        }

        [Fact]
        public void ReturnStart()
        {
            // Arrange
            statistics.ReportReceivedBatch(1, new long[1]);

            // Act
            var got = statistics.Start;

            // Assert
            got.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(99)]
        public void ReturnBatchCount(int batchCount)
        {
            // Arrange
            for (var i = 0; i < batchCount; i++)
            {
                statistics.ReportReceivedBatch(1, new long[1]);
            }

            // Act
            var got = statistics.BatchCount;

            // Assert
            got.ShouldBe(batchCount);
        }

        [Theory]
        [InlineData(0, null, null)]
        [InlineData(1, 0.1, 10)]
        [InlineData(2, 0.1, 20)]
        [InlineData(10, 0.5, 20)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 0.5)]
        [InlineData(10, 2, 5)]
        public void ReturnBatchesPerMinute(int batchCount, double? minutes, double? want)
        {
            // Arrange
            var now = new DateTime(2020, 1, 1);
            clock.Setup(mock => mock.Now).Returns(now);

            for (var i = 0; i < batchCount; i++)
            {
                statistics.ReportReceivedBatch(1, new long[1]);
            }

            if (minutes != null)
            {
                statistics.Start = now.AddMinutes(-(double)minutes);
            }

            // Act
            var got = statistics.BatchesPerMinute;

            // Assert
            got.ShouldBe(want);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 10, 10)]
        [InlineData(2, 10, 20)]
        [InlineData(4, 5, 20)]
        public void ReturnLogEventCount(int batchCount, int logEventsPerBatch, int want)
        {
            // Arrange
            for (var i = 0; i < batchCount; i++)
            {
                statistics.ReportReceivedBatch(1, new long[logEventsPerBatch]);
            }

            // Act
            var got = statistics.LogEventCount;

            // Assert
            got.ShouldBe(want);
        }

        [Theory]
        [InlineData(0, null, null)]
        [InlineData(1, 0.1, 10)]
        [InlineData(2, 0.1, 20)]
        [InlineData(10, 0.5, 20)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 0.5)]
        [InlineData(10, 2, 5)]
        public void ReturnLogEventsPerMinute(int logEvents, double? minutes, double? want)
        {
            // Arrange
            var now = new DateTime(2020, 1, 1);
            clock.Setup(mock => mock.Now).Returns(now);

            if (logEvents > 0)
            {
                statistics.ReportReceivedBatch(1, new long[logEvents]);
            }

            if (minutes != null)
            {
                statistics.Start = now.AddMinutes(-(double)minutes);
            }

            // Act
            var got = statistics.LogEventsPerMinute;

            // Assert
            got.ShouldBe(want);
        }
    }
}
