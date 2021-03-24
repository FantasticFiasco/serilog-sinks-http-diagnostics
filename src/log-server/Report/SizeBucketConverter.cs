namespace LogServer.Report
{
    public static class SizeBucketConverter
    {
        public static SizeBucket From(int logEventSize)
        {
            if (logEventSize < 512 * ByteSize.B) return SizeBucket.Below512B;
            if (logEventSize < 1 * ByteSize.KB) return SizeBucket.Between512BAnd1KB;
            if (logEventSize < 5 * ByteSize.KB) return SizeBucket.Between1And5KB;
            if (logEventSize < 10 * ByteSize.KB) return SizeBucket.Between5And10KB;
            if (logEventSize < 50 * ByteSize.KB) return SizeBucket.Between10And50KB;
            if (logEventSize < 100 * ByteSize.KB) return SizeBucket.Between50And100KB;
            if (logEventSize < 512 * ByteSize.KB) return SizeBucket.Between100And512KB;
            if (logEventSize < 1 * ByteSize.MB) return SizeBucket.Between512KBAnd1MB;
            if (logEventSize < 5 * ByteSize.MB) return SizeBucket.Between1And5MB;

            return SizeBucket.EqualToAndAbove5MB;
        }
    }
}
