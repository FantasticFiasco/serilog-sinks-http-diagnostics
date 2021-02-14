using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace LogServer.Stats
{
    public class Statistics
    {
        private readonly ConcurrentDictionary<Bucket, int> distribution;
        private long batchCount;
        private long eventCount;

        public Statistics()
        {
            distribution = new ConcurrentDictionary<Bucket, int>();
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

        public long EventCount
        {
            get { return Interlocked.Read(ref eventCount); }
        }

        public double? EventsPerMinute
        {
            get
            {
                var duration = Duration();

                return duration != null
                    ? (double)Interlocked.Read(ref eventCount) / ((TimeSpan)duration).TotalSeconds
                    : null;
            }
        }

        public void AddBatch(string[] logEvents)
        {
            if (Start == null)
            {
                Start = DateTime.Now;
            }

            Interlocked.Increment(ref batchCount);
            Interlocked.Add(ref eventCount, logEvents.Length);

            foreach (var logEvent in logEvents)
            {
                var bucket = BelongsTo(logEvent);
                distribution.AddOrUpdate(bucket, 1, (key, oldValue) => oldValue + 1);
            }
        }

        private TimeSpan? Duration()
        {
            var start = Start;

            return start != null
                ? DateTime.Now.Subtract((DateTime)start)
                : null;
        }

        private static Bucket BelongsTo(string logEvent)
        {
            var size = ByteSize.From(logEvent);

            if (size < 1 * ByteSize.KB) return Bucket.Below1KB;
            if (size < 5 * ByteSize.KB) return Bucket.Between1And5KB;
            if (size < 10 * ByteSize.KB) return Bucket.Between5And10KB;
            if (size < 50 * ByteSize.KB) return Bucket.Between10And50KB;
            if (size < 100 * ByteSize.KB) return Bucket.Between50And100KB;
            if (size < 500 * ByteSize.KB) return Bucket.Between100And500KB;
            if (size < 1 * ByteSize.MB) return Bucket.Between500KBAnd1MB;
            if (size < 5 * ByteSize.MB) return Bucket.Between1And5MB;

            return Bucket.Above5MB;
        }
    }
}
