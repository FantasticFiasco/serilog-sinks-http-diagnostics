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
        public void ReturnNullStartGivenNoReportedBatches()
        {
            // Assert
            statistics.Start.ShouldBeNull();
        }

        [Fact]
        public void ReturnStartGivenReportedBatches()
        {
            // Arrange
            statistics.ReportReceivedBatch(ByteSize.KB, new[] { ByteSize.KB });

            // Assert
            statistics.Start.ShouldNotBeNull();
        }

        [Fact]
        public void ReturnZeroBatchCountGivenNoReportedBatches()
        {
            // Assert
            statistics.BatchCount.ShouldBe(0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(99)]
        public void ReturnBatchCountGivenReportedBatches(int batchCount)
        {
            // Arrange
            for (var i = 0; i < batchCount; i++)
            {
                statistics.ReportReceivedBatch(1, new long[] { 1 });
            }

            // Assert
            statistics.BatchCount.ShouldBe(batchCount);
        }

        [Theory]
        [InlineData(0, null, null)]
        [InlineData(1, 0.1, 10)]
        [InlineData(2, 0.1, 20)]
        [InlineData(10, 0.5, 20)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 0.5)]
        [InlineData(10, 2, 5)]
        public void ReturnBatchesPerMinute(int batchCount, double? minutes, double? expected)
        {
            // Arrange
            var now = new DateTime(2020, 1, 1);
            clock.Setup(mock => mock.Now).Returns(now);

            for (var i = 0; i < batchCount; i++)
            {
                statistics.ReportReceivedBatch(1, new long[] { 1 });
            }

            if (minutes != null)
            {
                statistics.Start = now.AddMinutes(-(double)minutes);
            }

            // Assert
            statistics.BatchesPerMinute.ShouldBe(expected);
        }
    }
}
