using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace LogServer
{
    public class Statistics
    {
        //           x < 1 kB
        // 1 kB   <= x < 5 kB
        // 5 kB   <= x < 10  kB
        // 10 kB  <= x < 50  kB
        // 50 kB  <= x < 100 kB
        // 100 kB <= x < 500 kB
        // 500 kB <= x < 1 MB
        // 1 MB   <= x < 5 MB
        // 5 MB   <= x

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
