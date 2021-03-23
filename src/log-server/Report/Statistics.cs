using System;
using System.Collections.Concurrent;
using LogServer.Time;

namespace LogServer.Report
{
    public class Statistics
    {
        private readonly Clock _clock;
        private readonly ConcurrentDictionary<SizeBucket, int> _batchDistribution;
        private readonly ConcurrentDictionary<SizeBucket, int> _logEventDistribution;

        public Statistics(Clock clock)
        {
            _clock = clock;

            ContentLength = new MinMaxAverage();
            BatchSize = new MinMaxAverage();
            LogEventsPerBatch = new MinMaxAverage();
            LogEventSize = new MinMaxAverage();
            _batchDistribution = new ConcurrentDictionary<SizeBucket, int>();
            _logEventDistribution = new ConcurrentDictionary<SizeBucket, int>();
        }

        public DateTime? Start { get; set; }

        public string? ContentType { get; private set; }

        public string? ContentEncoding { get; private set; }

        public MinMaxAverage ContentLength { get; }

        public double? RequestsPerSecond
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? ContentLength.Count / ((TimeSpan)duration).TotalSeconds
                    : null;
            }
        }

        public MinMaxAverage BatchSize { get; }

        public double CompressionRatio => BatchSize.Average / ContentLength.Average;

        public MinMaxAverage LogEventsPerBatch { get; }

        public MinMaxAverage LogEventSize { get; }

        public double? LogEventsPerSecond
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? LogEventSize.Count / ((TimeSpan)duration).TotalSeconds
                    : null;
            }
        }
        public void ReportReceivedBatch(
            string contentType,
            string contentEncoding,
            int contentLength,
            int batchSize,
            int[] logEventSizes)
        {
            if (Start == null)
            {
                Start = _clock.Now;
            }

            // Request
            ContentType = contentType;
            ContentEncoding = contentEncoding;
            ContentLength.Update(contentLength);

            // Batch
            BatchSize.Update(batchSize);
            _batchDistribution.AddOrUpdate(
                SizeBucketConverter.From(batchSize),
                1,
                (key, oldValue) => oldValue + 1);

            // Log events
            LogEventsPerBatch.Update(logEventSizes.Length);

            foreach (var logEventSize in logEventSizes)
            {
                LogEventSize.Update(logEventSize);

                var sizeBucket = SizeBucketConverter.From(logEventSize);
                _logEventDistribution.AddOrUpdate(sizeBucket, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public int BatchesOfSize(SizeBucket sizeBucket)
        {
            var success = _batchDistribution.TryGetValue(sizeBucket, out var count);
            return success ? count : 0;
        }

        public int LogEventsOfSize(SizeBucket sizeBucket)
        {
            var success = _logEventDistribution.TryGetValue(sizeBucket, out var count);
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
