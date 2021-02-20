using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly ConcurrentBag<int> batchSizes;
        private readonly ConcurrentDictionary<LogEventSize, int> logEventDistribution;
        private long logEventCount;

        public Statistics()
        {
            batchSizes = new ConcurrentBag<int>();
            logEventDistribution = new ConcurrentDictionary<LogEventSize, int>();
        }

        public DateTime? Start { get; set; }

        public long BatchCount
        {
            get { return batchSizes.Count; }
        }

        public double? BatchesPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? (double)BatchCount / ((TimeSpan)duration).TotalSeconds
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

        public void ReportReceivedBatch(int batchSize, int[] logEventSizes)
        {
            if (Start == null)
            {
                Start = DateTime.Now;
            }

            batchSizes.Add(batchSize);
            Interlocked.Add(ref logEventCount, logEventSizes.Length);

            foreach (var logEventSize in logEventSizes)
            {
                var size = LogEventSize.From(logEventSize);
                logEventDistribution.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public int NbrOfLogEvents(LogEventSize size)
        {
            bool success = logEventDistribution.TryGetValue(size, out int count);
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
