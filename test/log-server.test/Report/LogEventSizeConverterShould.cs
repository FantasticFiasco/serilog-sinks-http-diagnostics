using Shouldly;
using Xunit;

namespace LogServer.Report
{
    public class LogEventSizeConverterShould
    {
        [Theory]
        [InlineData(1, SizeBucket.Below512B)]
        [InlineData(512 * ByteSize.B - 1, SizeBucket.Below512B)]
        [InlineData(512 * ByteSize.B, SizeBucket.Between512BAnd1KB)]
        [InlineData(1 * ByteSize.KB - 1, SizeBucket.Between512BAnd1KB)]
        [InlineData(1 * ByteSize.KB, SizeBucket.Between1And5KB)]
        [InlineData(5 * ByteSize.KB - 1, SizeBucket.Between1And5KB)]
        [InlineData(5 * ByteSize.KB, SizeBucket.Between5And10KB)]
        [InlineData(10 * ByteSize.KB - 1, SizeBucket.Between5And10KB)]
        [InlineData(10 * ByteSize.KB, SizeBucket.Between10And50KB)]
        [InlineData(50 * ByteSize.KB - 1, SizeBucket.Between10And50KB)]
        [InlineData(50 * ByteSize.KB, SizeBucket.Between50And100KB)]
        [InlineData(100 * ByteSize.KB - 1, SizeBucket.Between50And100KB)]
        [InlineData(100 * ByteSize.KB, SizeBucket.Between100And512KB)]
        [InlineData(512 * ByteSize.KB - 1, SizeBucket.Between100And512KB)]
        [InlineData(512 * ByteSize.KB, SizeBucket.Between512KBAnd1MB)]
        [InlineData(1 * ByteSize.MB - 1, SizeBucket.Between512KBAnd1MB)]
        [InlineData(1 * ByteSize.MB, SizeBucket.Between1And5MB)]
        [InlineData(5 * ByteSize.MB - 1, SizeBucket.Between1And5MB)]
        [InlineData(5 * ByteSize.MB, SizeBucket.EqualToAndAbove5MB)]
        [InlineData(10 * ByteSize.MB, SizeBucket.EqualToAndAbove5MB)]
        public void ReturnLogEventSize(int logEventSize, SizeBucket want)
        {
            // Act
            var got = SizeBucketConverter.From(logEventSize);

            // Assert
            got.ShouldBe(want);
        }
    }
}
