using System;
using System.Collections.Concurrent;
using LogServer.Time;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly Clock _clock;
        private readonly ConcurrentDictionary<LogEventSize, int> _logEventDistribution;

        public Statistics(Clock clock)
        {
            _clock = clock;

            BatchSize = new MinMaxAverage();
            LogEventsPerBatch = new MinMaxAverage();
            LogEventSize = new MinMaxAverage();
            _logEventDistribution = new ConcurrentDictionary<LogEventSize, int>();
        }

        public DateTime? Start { get; set; }

        public string ContentType { get; private set; }

        public string ContentEncoding { get; private set; }

        public MinMaxAverage BatchSize { get; }

        public double? BatchesPerSecond
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? 1000.0 * BatchSize.Count / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }

        public MinMaxAverage LogEventsPerBatch { get; }

        public MinMaxAverage LogEventSize { get; }

        public double? LogEventsPerSecond
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? 1000.0 * LogEventSize.Count / ((TimeSpan)duration).TotalMilliseconds
                    : null;
            }
        }
        public void ReportReceivedBatch(string contentType, string contentEncoding, int batchSize, int[] logEventSizes)
        {
            if (Start == null)
            {
                Start = _clock.Now;
            }

            ContentType = contentType;
            ContentEncoding = contentEncoding;

            BatchSize.Update(batchSize);
            LogEventsPerBatch.Update(logEventSizes.Length);

            foreach (var logEventSize in logEventSizes)
            {
                LogEventSize.Update(logEventSize);

                var size = LogEventSizeConverter.From(logEventSize);
                _logEventDistribution.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public int LogEventsOfSize(LogEventSize size)
        {
            var success = _logEventDistribution.TryGetValue(size, out var count);
            return success ? count : 0;
        }

        private TimeSpan? Duration()
        {
            var start = Start;

            return start != null
                ? _clock.Now.Subtract((DateTime)start)
                : null;
        }
    }
}
