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
                var start = Start;

                if (start == null)
                {
                    return null;
                }

                return (double)Interlocked.Read(ref batchCount) / DateTime.Now.Subtract((DateTime)start).TotalSeconds;
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
                var start = Start;

                if (start == null)
                {
                    return null;
                }

                return (double)Interlocked.Read(ref eventCount) / DateTime.Now.Subtract((DateTime)start).TotalSeconds;
            }
        }

        public void AddBatch(string[] logEvents)
        {
            var now = DateTime.Now;

            if (Start == null)
            {
                Start = now;
            }

            var currentBatchCount = Interlocked.Increment(ref batchCount);

            var currentEventCount = Interlocked.Add(ref eventCount, logEvents.Length);

            foreach (var logEvent in logEvents)
            {
                var size = UTF8Encoding.UTF8.GetByteCount(logEvent);
                data.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }
    }
}
