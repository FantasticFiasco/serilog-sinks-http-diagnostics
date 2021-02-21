using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using LogServer.Time;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly Clock clock;
        private readonly ConcurrentBag<int> batchSizes;
        private readonly ConcurrentDictionary<LogEventSize, int> logEventDistribution;

        private long minLogEventSize;
        private long maxLogEventSize;

        public Statistics(Clock clock)
        {
            this.clock = clock;

            batchSizes = new ConcurrentBag<int>();
            logEventDistribution = new ConcurrentDictionary<LogEventSize, int>();
        }

        public DateTime? Start { get; set; }

        public int BatchCount
        {
            get { return batchSizes.Count; }
        }

        public int MinBatchSize
        {
            get { return batchSizes.Min(); }
        }

        public int MaxBatchSize
        {
            get { return batchSizes.Max(); }
        }

        public double AverageBatchSize
        {
            get { return batchSizes.Average(); }
        }

        public double? BatchesPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? 60000.0 * BatchCount / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }

        public int LogEventCount
        {
            get { return logEventDistribution.Values.Sum(); }
        }

        public int MinLogEventSize
        {
            get { return (int)Interlocked.Read(ref minLogEventSize); }
            private set { Interlocked.Exchange(ref minLogEventSize, (long)value); }
        }

        public int MaxLogEventSize
        {
            get { return (int)Interlocked.Read(ref maxLogEventSize); }
            private set { Interlocked.Exchange(ref maxLogEventSize, (long)value); }
        }

        public double? LogEventsPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? 60000.0 * LogEventCount / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }

        public void ReportReceivedBatch(int batchSize, int[] logEventSizes)
        {
            if (Start == null)
            {
                Start = clock.Now;
            }

            batchSizes.Add(batchSize);

            foreach (var logEventSize in logEventSizes)
            {
                if (MinLogEventSize == 0 || logEventSize < MinLogEventSize)
                {
                    MinLogEventSize = logEventSize;
                }

                if (MaxLogEventSize == 0 || logEventSize > MaxLogEventSize)
                {
                    MaxLogEventSize = logEventSize;
                }

                var size = LogEventSizeConverter.From(logEventSize);
                logEventDistribution.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public int LogEventsOfSize(LogEventSize size)
        {
            bool success = logEventDistribution.TryGetValue(size, out int count);
            return success ? count : 0;
        }

        private TimeSpan? Duration()
        {
            var start = Start;

            return start != null
                ? clock.Now.Subtract((DateTime)start)
                : null;
        }
    }
}
