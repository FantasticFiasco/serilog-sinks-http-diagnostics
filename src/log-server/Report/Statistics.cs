using System;
using System.Collections.Concurrent;
using LogServer.Time;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly Clock clock;
        private readonly ConcurrentDictionary<LogEventSize, int> logEventDistribution;

        public Statistics(Clock clock)
        {
            this.clock = clock;

            BatchSize = new MinMaxAverage();
            LogEventSize = new MinMaxAverage();
            logEventDistribution = new ConcurrentDictionary<LogEventSize, int>();
        }

        public DateTime? Start { get; set; }

        public MinMaxAverage BatchSize { get; }

        public double? BatchesPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? 60000.0 * BatchSize.Count / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }

        public MinMaxAverage LogEventSize { get; }

        public double? LogEventsPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? 60000.0 * LogEventSize.Count / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }

        public void ReportReceivedBatch(int batchSize, int[] logEventSizes)
        {
            if (Start == null)
            {
                Start = clock.Now;
            }

            BatchSize.Update(batchSize);

            foreach (var logEventSize in logEventSizes)
            {
                LogEventSize.Update(logEventSize);

                var size = LogEventSizeConverter.From(logEventSize);
                logEventDistribution.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public int LogEventsOfSize(LogEventSize size)
        {
            var success = logEventDistribution.TryGetValue(size, out var count);
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
