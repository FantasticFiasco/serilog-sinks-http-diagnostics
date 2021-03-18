namespace LogServer.Report
{
    public static class LogEventSizeConverter
    {
        public static LogEventSize From(int logEventSize)
        {
            if (logEventSize < 512 * ByteSize.B) return LogEventSize.Below512B;
            if (logEventSize < 1 * ByteSize.Kb) return LogEventSize.Between512BAnd1Kb;
            if (logEventSize < 5 * ByteSize.Kb) return LogEventSize.Between1And5Kb;
            if (logEventSize < 10 * ByteSize.Kb) return LogEventSize.Between5And10Kb;
            if (logEventSize < 50 * ByteSize.Kb) return LogEventSize.Between10And50Kb;
            if (logEventSize < 100 * ByteSize.Kb) return LogEventSize.Between50And100Kb;
            if (logEventSize < 512 * ByteSize.Kb) return LogEventSize.Between100And512Kb;
            if (logEventSize < 1 * ByteSize.Mb) return LogEventSize.Between512KbAnd1Mb;
            if (logEventSize < 5 * ByteSize.Mb) return LogEventSize.Between1And5Mb;

            return LogEventSize.EqualToAndAbove5Mb;
        }
    }
}
