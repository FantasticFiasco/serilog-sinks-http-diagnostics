using System;
using System.Collections.Concurrent;
using System.Linq;
using LogServer.Time;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly Clock clock;
        private readonly ConcurrentBag<int> batchSizes;
        private readonly ConcurrentDictionary<LogEventSize, int> logEventDistribution;

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
