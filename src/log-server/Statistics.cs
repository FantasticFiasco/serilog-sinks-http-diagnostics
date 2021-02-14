using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace LogServer
{
    public class Statistics
    {
        private readonly ConcurrentDictionary<int, int> data;
        private long batchCount;
        private long eventCount;

        public Statistics()
        {
            data = new ConcurrentDictionary<int, int>();
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
                var size = UTF8Encoding.UTF8.GetByteCount(logEvent);
                data.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
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
