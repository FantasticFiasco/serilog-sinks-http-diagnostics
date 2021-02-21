using Shouldly;
using Xunit;

namespace LogServer.Report
{
    public class LogEventSizeConverterShould
    {
        [Theory]
        [InlineData(1, LogEventSize.Below512B)]
        [InlineData(512 * ByteSize.B - 1, LogEventSize.Below512B)]
        [InlineData(512 * ByteSize.B, LogEventSize.Between512BAnd1KB)]
        [InlineData(1 * ByteSize.KB - 1, LogEventSize.Between512BAnd1KB)]
        [InlineData(1 * ByteSize.KB, LogEventSize.Between1And5KB)]
        [InlineData(5 * ByteSize.KB - 1, LogEventSize.Between1And5KB)]
        [InlineData(5 * ByteSize.KB, LogEventSize.Between5And10KB)]
        [InlineData(10 * ByteSize.KB - 1, LogEventSize.Between5And10KB)]
        [InlineData(10 * ByteSize.KB, LogEventSize.Between10And50KB)]
        [InlineData(50 * ByteSize.KB - 1, LogEventSize.Between10And50KB)]
        [InlineData(50 * ByteSize.KB, LogEventSize.Between50And100KB)]
        [InlineData(100 * ByteSize.KB - 1, LogEventSize.Between50And100KB)]
        [InlineData(100 * ByteSize.KB, LogEventSize.Between100And512KB)]
        [InlineData(512 * ByteSize.KB - 1, LogEventSize.Between100And512KB)]
        [InlineData(512 * ByteSize.KB, LogEventSize.Between512KBAnd1MB)]
        [InlineData(1 * ByteSize.MB - 1, LogEventSize.Between512KBAnd1MB)]
        [InlineData(1 * ByteSize.MB, LogEventSize.Between1And5MB)]
        [InlineData(5 * ByteSize.MB - 1, LogEventSize.Between1And5MB)]
        [InlineData(5 * ByteSize.MB, LogEventSize.EqualToAndAbove5MB)]
        [InlineData(10 * ByteSize.MB, LogEventSize.EqualToAndAbove5MB)]
        public void ReturnLogEventSize(long logEventSize, LogEventSize want)
        {
            // Act
            var got = LogEventSizeConverter.From(logEventSize);

            // Assert
            got.ShouldBe(want);
        }
    }
}
