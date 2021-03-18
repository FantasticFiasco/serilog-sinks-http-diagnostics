using Shouldly;
using Xunit;

namespace LogServer.Report
{
    public class LogEventSizeConverterShould
    {
        [Theory]
        [InlineData(1, LogEventSize.Below512B)]
        [InlineData(512 * ByteSize.B - 1, LogEventSize.Below512B)]
        [InlineData(512 * ByteSize.B, LogEventSize.Between512BAnd1Kb)]
        [InlineData(1 * ByteSize.KB - 1, LogEventSize.Between512BAnd1Kb)]
        [InlineData(1 * ByteSize.KB, LogEventSize.Between1And5Kb)]
        [InlineData(5 * ByteSize.KB - 1, LogEventSize.Between1And5Kb)]
        [InlineData(5 * ByteSize.KB, LogEventSize.Between5And10Kb)]
        [InlineData(10 * ByteSize.KB - 1, LogEventSize.Between5And10Kb)]
        [InlineData(10 * ByteSize.KB, LogEventSize.Between10And50Kb)]
        [InlineData(50 * ByteSize.KB - 1, LogEventSize.Between10And50Kb)]
        [InlineData(50 * ByteSize.KB, LogEventSize.Between50And100Kb)]
        [InlineData(100 * ByteSize.KB - 1, LogEventSize.Between50And100Kb)]
        [InlineData(100 * ByteSize.KB, LogEventSize.Between100And512Kb)]
        [InlineData(512 * ByteSize.KB - 1, LogEventSize.Between100And512Kb)]
        [InlineData(512 * ByteSize.KB, LogEventSize.Between512KbAnd1Mb)]
        [InlineData(1 * ByteSize.MB - 1, LogEventSize.Between512KbAnd1Mb)]
        [InlineData(1 * ByteSize.MB, LogEventSize.Between1And5Mb)]
        [InlineData(5 * ByteSize.MB - 1, LogEventSize.Between1And5Mb)]
        [InlineData(5 * ByteSize.MB, LogEventSize.EqualToAndAbove5Mb)]
        [InlineData(10 * ByteSize.MB, LogEventSize.EqualToAndAbove5Mb)]
        public void ReturnLogEventSize(int logEventSize, LogEventSize want)
        {
            // Act
            var got = LogEventSizeConverter.From(logEventSize);

            // Assert
            got.ShouldBe(want);
        }
    }
}
