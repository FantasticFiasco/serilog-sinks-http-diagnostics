namespace LogServer.Report
{
    public enum LogEventSize
    {
        Below512B,
        Between512BAnd1KB,
        Between1And5KB,
        Between5And10KB,
        Between10And50KB,
        Between50And100KB,
        Between100And512KB,
        Between512KBAnd1MB,
        Between1And5MB,
        Above5MB
    }

    public static class LogEventSizeConverter
    {
        public static LogEventSize From(long logEventSize)
        {
            if (logEventSize < 512 * ByteSize.B) return LogEventSize.Below512B;
            if (logEventSize < 1 * ByteSize.KB) return LogEventSize.Between512BAnd1KB;
            if (logEventSize < 5 * ByteSize.KB) return LogEventSize.Between1And5KB;
            if (logEventSize < 10 * ByteSize.KB) return LogEventSize.Between5And10KB;
            if (logEventSize < 50 * ByteSize.KB) return LogEventSize.Between10And50KB;
            if (logEventSize < 100 * ByteSize.KB) return LogEventSize.Between50And100KB;
            if (logEventSize < 512 * ByteSize.KB) return LogEventSize.Between100And512KB;
            if (logEventSize < 1 * ByteSize.MB) return LogEventSize.Between512KBAnd1MB;
            if (logEventSize < 5 * ByteSize.MB) return LogEventSize.Between1And5MB;

            return LogEventSize.Above5MB;
        }
    }
}
