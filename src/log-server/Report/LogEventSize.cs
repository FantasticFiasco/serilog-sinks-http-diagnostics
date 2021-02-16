namespace LogServer.Report
{
    public class LogEventSize
    {
        private LogEventSize()
        {
        }

        public static readonly LogEventSize Below512B = new LogEventSize();
        public static readonly LogEventSize Between512BAnd1KB = new LogEventSize();
        public static readonly LogEventSize Between1And5KB = new LogEventSize();
        public static readonly LogEventSize Between5And10KB = new LogEventSize();
        public static readonly LogEventSize Between10And50KB = new LogEventSize();
        public static readonly LogEventSize Between50And100KB = new LogEventSize();
        public static readonly LogEventSize Between100And512KB = new LogEventSize();
        public static readonly LogEventSize Between512KBAnd1MB = new LogEventSize();
        public static readonly LogEventSize Between1And5MB = new LogEventSize();
        public static readonly LogEventSize Above5MB = new LogEventSize();

        public static LogEventSize From(string logEvent)
        {
            var size = ByteSize.From(logEvent);

            if (size < 512 * ByteSize.B) return Below512B;
            if (size < 1 * ByteSize.KB) return Between512BAnd1KB;
            if (size < 5 * ByteSize.KB) return Between1And5KB;
            if (size < 10 * ByteSize.KB) return Between5And10KB;
            if (size < 50 * ByteSize.KB) return Between10And50KB;
            if (size < 100 * ByteSize.KB) return Between50And100KB;
            if (size < 512 * ByteSize.KB) return Between100And512KB;
            if (size < 1 * ByteSize.MB) return Between512KBAnd1MB;
            if (size < 5 * ByteSize.MB) return Between1And5MB;

            return Above5MB;
        }
    }
}
