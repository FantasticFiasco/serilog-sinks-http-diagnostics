using System;
using System.Collections.Concurrent;
using System.Linq;
using LogServer.Time;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly Clock clock;
        private readonly ConcurrentBag<long> batchSizes;
        private readonly ConcurrentDictionary<LogEventSize, long> logEventDistribution;

        public Statistics(Clock clock)
        {
            this.clock = clock;

            batchSizes = new ConcurrentBag<long>();
            logEventDistribution = new ConcurrentDictionary<LogEventSize, long>();
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
                    ? 60000.0 * BatchCount / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }

        public long LogEventCount
        {
            get { return logEventDistribution.Values.Sum(); }
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

        public void ReportReceivedBatch(long batchSize, long[] logEventSizes)
        {
            if (Start == null)
            {
                Start = clock.Now;
            }

            batchSizes.Add(batchSize);

            foreach (var logEventSize in logEventSizes)
            {
                var size = LogEventSize.From(logEventSize);
                logEventDistribution.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public long LogEventsOfSize(LogEventSize size)
        {
            bool success = logEventDistribution.TryGetValue(size, out long count);
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
