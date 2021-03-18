namespace LogServer.Report
{
    public static class LogEventSizeConverter
    {
        public static LogEventSize From(int logEventSize)
        {
            if (logEventSize < 512 * ByteSize.B) return LogEventSize.Below512B;
            if (logEventSize < 1 * ByteSize.KB) return LogEventSize.Between512BAnd1Kb;
            if (logEventSize < 5 * ByteSize.KB) return LogEventSize.Between1And5Kb;
            if (logEventSize < 10 * ByteSize.KB) return LogEventSize.Between5And10Kb;
            if (logEventSize < 50 * ByteSize.KB) return LogEventSize.Between10And50Kb;
            if (logEventSize < 100 * ByteSize.KB) return LogEventSize.Between50And100Kb;
            if (logEventSize < 512 * ByteSize.KB) return LogEventSize.Between100And512Kb;
            if (logEventSize < 1 * ByteSize.MB) return LogEventSize.Between512KbAnd1Mb;
            if (logEventSize < 5 * ByteSize.MB) return LogEventSize.Between1And5Mb;

            return LogEventSize.EqualToAndAbove5Mb;
        }
    }
}
