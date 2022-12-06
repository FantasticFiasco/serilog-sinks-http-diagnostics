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
        private readonly Mock<Clock> _clock;
        private readonly Statistics _statistics;

        public StatisticsShould()
        {
            _clock = new Mock<Clock>();
            _statistics = new Statistics(_clock.Object);
        }

        [Fact]
        public void InitiallyBe()
        {
            // Assert
            _statistics.Start.ShouldBeNull();

            _statistics.RequestsPerSecond.ShouldBeNull();

            _statistics.BatchSize.Min.ShouldBe(0);
            _statistics.BatchSize.Max.ShouldBe(0);
            _statistics.BatchSize.Average.ShouldBe(0.0);
            _statistics.BatchSize.Count.ShouldBe(0);

            _statistics.LogEventSize.Min.ShouldBe(0);
            _statistics.LogEventSize.Max.ShouldBe(0);
            _statistics.LogEventSize.Average.ShouldBe(0);
            _statistics.LogEventSize.Count.ShouldBe(0);

            _statistics.LogEventsPerSecond.ShouldBeNull();
        }

        [Fact]
        public void ReturnStart()
        {
            // Arrange
            _statistics.ReportReceivedBatch("", "", 1, 1, new int[1]);

            // Act
            var got = _statistics.Start;

            // Assert
            got.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(0, null, null)]
        [InlineData(1, 0.1, 10)]
        [InlineData(2, 0.1, 20)]
        [InlineData(10, 0.5, 20)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 0.5)]
        [InlineData(10, 2, 5)]
        public void ReturnBatchesPerSecond(int batchCount, double? seconds, double? want)
        {
            // Arrange
            var now = new DateTime(2020, 1, 1);
            _clock.Setup(mock => mock.Now).Returns(now);

            for (var i = 0; i < batchCount; i++)
            {
                _statistics.ReportReceivedBatch("", "", 1, 1, new int[1]);
            }

            if (seconds != null)
            {
                _statistics.Start = now.AddSeconds(-(double)seconds);
            }

            // Act
            var got = _statistics.RequestsPerSecond;

            // Assert
            got.ShouldBe(want);
        }

        [Theory]
        [InlineData(0, null, null)]
        [InlineData(1, 0.1, 10)]
        [InlineData(2, 0.1, 20)]
        [InlineData(10, 0.5, 20)]
        [InlineData(1, 1.0, 1.0)]
        [InlineData(1, 2.0, 0.5)]
        [InlineData(10, 2.0, 5.0)]
        public void ReturnLogEventsPerSecond(int logEvents, double? seconds, double? want)
        {
            // Arrange
            var now = new DateTime(2020, 1, 1);
            _clock.Setup(mock => mock.Now).Returns(now);

            if (logEvents > 0)
            {
                _statistics.ReportReceivedBatch("", "", 1, 1, new int[logEvents]);
            }

            if (seconds != null)
            {
                _statistics.Start = now.AddSeconds(-(double)seconds);
            }

            // Act
            var got = _statistics.LogEventsPerSecond;

            // Assert
            got.ShouldBe(want);
        }

        [Theory]
        [InlineData(SizeBucket.Below512B, 2 * 1)]
        [InlineData(SizeBucket.Between512BAnd1KB, 2 * 2)]
        [InlineData(SizeBucket.Between1And5KB, 2 * 3)]
        [InlineData(SizeBucket.Between5And10KB, 2 * 4)]
        [InlineData(SizeBucket.Between10And50KB, 2 * 5)]
        [InlineData(SizeBucket.Between50And100KB, 2 * 6)]
        [InlineData(SizeBucket.Between100And512KB, 2 * 7)]
        [InlineData(SizeBucket.Between512KBAnd1MB, 2 * 8)]
        [InlineData(SizeBucket.Between1And5MB, 2 * 9)]
        [InlineData(SizeBucket.EqualToAndAbove5MB, 2 * 10)]
        public void ReturnLogEventsOfSize(SizeBucket sizeBucket, int want)
        {
            // Arrange
            // Below512B
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(1, 1 * ByteSize.B));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(1, 512 * ByteSize.B - 1));

            // Between512BAnd1KB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(2, 512 * ByteSize.B));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(2, 1 * ByteSize.KB - 1));

            // Between1And5KB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(3, 1 * ByteSize.KB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(3, 5 * ByteSize.KB - 1));

            // Between5And10KB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(4, 5 * ByteSize.KB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(4, 10 * ByteSize.KB - 1));

            // Between10And50KB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(5, 10 * ByteSize.KB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(5, 50 * ByteSize.KB - 1));

            // Between50And100KB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(6, 50 * ByteSize.KB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(6, 100 * ByteSize.KB - 1));

            // Between100And512KB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(7, 100 * ByteSize.KB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(7, 512 * ByteSize.KB - 1));

            // Between512KBAnd1MB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(8, 512 * ByteSize.KB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(8, 1 * ByteSize.MB - 1));

            // Between1And5MB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(9, 1 * ByteSize.MB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(9, 5 * ByteSize.MB - 1));

            // EqualToAndAbove5MB
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(10, 5 * ByteSize.MB));
            _statistics.ReportReceivedBatch("", "", 1, 1, Repeat(10, 10 * ByteSize.MB));

            // Act
            var got = _statistics.LogEventsOfSize(sizeBucket);

            // Assert
            got.ShouldBe(want);
        }

        private static int[] Repeat(int count, int value)
        {
            return Enumerable
                .Range(0, count)
                .Select(_ => value)
                .ToArray();
        }
    }
}
