using System;
using System.Linq;
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

            statistics.BatchSize.Min.ShouldBe(0);
            statistics.BatchSize.Max.ShouldBe(0);
            statistics.BatchSize.Average.ShouldBe(0.0);
            statistics.BatchSize.Count.ShouldBe(0);

            statistics.BatchesPerMinute.ShouldBeNull();

            statistics.LogEventSize.Min.ShouldBe(0);
            statistics.LogEventSize.Max.ShouldBe(0);
            statistics.LogEventSize.Average.ShouldBe(0);
            statistics.LogEventSize.Count.ShouldBe(0);

            statistics.LogEventsPerMinute.ShouldBeNull();
        }

        [Fact]
        public void ReturnStart()
        {
            // Arrange
            statistics.ReportReceivedBatch(1, new int[1]);

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
                statistics.ReportReceivedBatch(1, new int[1]);
            }

            // Act
            var got = statistics.BatchSize.Count;

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
                statistics.ReportReceivedBatch(1, new int[1]);
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
                statistics.ReportReceivedBatch(1, new int[logEventsPerBatch]);
            }

            // Act
            var got = statistics.LogEventSize.Count;

            // Assert
            got.ShouldBe(want);
        }

        [Fact]
        public void ReturnMinLogEventSize()
        {
            // Arrange
            statistics.ReportReceivedBatch(1, new[] { 27, 11, 99, 25, 78 });

            // Act
            var got = statistics.LogEventSize.Min;

            // Assert
            got.ShouldBe(11);
        }

        [Fact]
        public void ReturnMaxLogEventSize()
        {
            // Arrange
            statistics.ReportReceivedBatch(1, new[] { 27, 11, 99, 25, 78 });

            // Act
            var got = statistics.LogEventSize.Max;

            // Assert
            got.ShouldBe(99);
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
                statistics.ReportReceivedBatch(1, new int[logEvents]);
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

        [Theory]
        [InlineData(LogEventSize.Below512B, 2 * 1)]
        [InlineData(LogEventSize.Between512BAnd1KB, 2 * 2)]
        [InlineData(LogEventSize.Between1And5KB, 2 * 3)]
        [InlineData(LogEventSize.Between5And10KB, 2 * 4)]
        [InlineData(LogEventSize.Between10And50KB, 2 * 5)]
        [InlineData(LogEventSize.Between50And100KB, 2 * 6)]
        [InlineData(LogEventSize.Between100And512KB, 2 * 7)]
        [InlineData(LogEventSize.Between512KBAnd1MB, 2 * 8)]
        [InlineData(LogEventSize.Between1And5MB, 2 * 9)]
        [InlineData(LogEventSize.EqualToAndAbove5MB, 2 * 10)]
        public void ReturnLogEventsOfSize(LogEventSize size, int want)
        {
            // Arrange
            // Below512B
            statistics.ReportReceivedBatch(1, Repeat(1, 1 * ByteSize.B));
            statistics.ReportReceivedBatch(1, Repeat(1, 512 * ByteSize.B - 1));

            // Between512BAnd1KB
            statistics.ReportReceivedBatch(1, Repeat(2, 512 * ByteSize.B));
            statistics.ReportReceivedBatch(1, Repeat(2, 1 * ByteSize.KB - 1));

            // Between1And5KB
            statistics.ReportReceivedBatch(1, Repeat(3, 1 * ByteSize.KB));
            statistics.ReportReceivedBatch(1, Repeat(3, 5 * ByteSize.KB - 1));

            // Between5And10KB
            statistics.ReportReceivedBatch(1, Repeat(4, 5 * ByteSize.KB));
            statistics.ReportReceivedBatch(1, Repeat(4, 10 * ByteSize.KB - 1));

            // Between10And50KB
            statistics.ReportReceivedBatch(1, Repeat(5, 10 * ByteSize.KB));
            statistics.ReportReceivedBatch(1, Repeat(5, 50 * ByteSize.KB - 1));

            // Between50And100KB
            statistics.ReportReceivedBatch(1, Repeat(6, 50 * ByteSize.KB));
            statistics.ReportReceivedBatch(1, Repeat(6, 100 * ByteSize.KB - 1));

            // Between100And512KB
            statistics.ReportReceivedBatch(1, Repeat(7, 100 * ByteSize.KB));
            statistics.ReportReceivedBatch(1, Repeat(7, 512 * ByteSize.KB - 1));

            // Between512KBAnd1MB
            statistics.ReportReceivedBatch(1, Repeat(8, 512 * ByteSize.KB));
            statistics.ReportReceivedBatch(1, Repeat(8, 1 * ByteSize.MB - 1));

            // Between1And5MB
            statistics.ReportReceivedBatch(1, Repeat(9, 1 * ByteSize.MB));
            statistics.ReportReceivedBatch(1, Repeat(9, 5 * ByteSize.MB - 1));

            // EqualToAndAbove5MB
            statistics.ReportReceivedBatch(1, Repeat(10, 5 * ByteSize.MB));
            statistics.ReportReceivedBatch(1, Repeat(10, 10 * ByteSize.MB));

            // Act
            var got = statistics.LogEventsOfSize(size);

            // Assert
            got.ShouldBe(want);
        }

        private int[] Repeat(int count, int value)
        {
            return Enumerable
                .Range(0, count)
                .Select(_ => value)
                .ToArray();
        }
    }
}
