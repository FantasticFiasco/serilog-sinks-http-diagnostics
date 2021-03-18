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
        [InlineData(1 * ByteSize.Kb - 1, LogEventSize.Between512BAnd1Kb)]
        [InlineData(1 * ByteSize.Kb, LogEventSize.Between1And5Kb)]
        [InlineData(5 * ByteSize.Kb - 1, LogEventSize.Between1And5Kb)]
        [InlineData(5 * ByteSize.Kb, LogEventSize.Between5And10Kb)]
        [InlineData(10 * ByteSize.Kb - 1, LogEventSize.Between5And10Kb)]
        [InlineData(10 * ByteSize.Kb, LogEventSize.Between10And50Kb)]
        [InlineData(50 * ByteSize.Kb - 1, LogEventSize.Between10And50Kb)]
        [InlineData(50 * ByteSize.Kb, LogEventSize.Between50And100Kb)]
        [InlineData(100 * ByteSize.Kb - 1, LogEventSize.Between50And100Kb)]
        [InlineData(100 * ByteSize.Kb, LogEventSize.Between100And512Kb)]
        [InlineData(512 * ByteSize.Kb - 1, LogEventSize.Between100And512Kb)]
        [InlineData(512 * ByteSize.Kb, LogEventSize.Between512KbAnd1Mb)]
        [InlineData(1 * ByteSize.Mb - 1, LogEventSize.Between512KbAnd1Mb)]
        [InlineData(1 * ByteSize.Mb, LogEventSize.Between1And5Mb)]
        [InlineData(5 * ByteSize.Mb - 1, LogEventSize.Between1And5Mb)]
        [InlineData(5 * ByteSize.Mb, LogEventSize.EqualToAndAbove5Mb)]
        [InlineData(10 * ByteSize.Mb, LogEventSize.EqualToAndAbove5Mb)]
        public void ReturnLogEventSize(int logEventSize, LogEventSize want)
        {
            // Act
            var got = LogEventSizeConverter.From(logEventSize);

            // Assert
            got.ShouldBe(want);
        }
    }
}
