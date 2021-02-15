using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly ConcurrentDictionary<LogEventSize, int> distribution;
        private long batchCount;
        private long logEventCount;

        public Statistics()
        {
            distribution = new ConcurrentDictionary<LogEventSize, int>();
        }

        public DateTime? Start { get; set; }

        public long BatchCount
        {
            get { return Interlocked.Read(ref batchCount); }
        }

        public double? BatchesPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? (double)Interlocked.Read(ref batchCount) / ((TimeSpan)duration).TotalSeconds
                    : null;
            }
        }

        public long LogEventCount
        {
            get { return Interlocked.Read(ref logEventCount); }
        }

        public double? LogEventsPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? (double)Interlocked.Read(ref logEventCount) / ((TimeSpan)duration).TotalSeconds
                    : null;
            }
        }

        public void ReportReceivedBatch(string[] logEvents)
        {
            if (Start == null)
            {
                Start = DateTime.Now;
            }

            Interlocked.Increment(ref batchCount);
            Interlocked.Add(ref logEventCount, logEvents.Length);

            foreach (var logEvent in logEvents)
            {
                var size = LogEventSize.From(logEvent);
                distribution.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public int NbrOfEvents(LogEventSize size)
        {
            bool success = distribution.TryGetValue(size, out int count);
            return success ? count : 0;
        }

        private TimeSpan? Duration()
        {
            var start = Start;

            return start != null
                ? DateTime.Now.Subtract((DateTime)start)
                : null;
        }
    }
}
