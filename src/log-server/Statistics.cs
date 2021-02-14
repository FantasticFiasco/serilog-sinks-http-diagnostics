using System;
using System.Collections.Concurrent;
using System.Text;

namespace LogServer
{
    public class Statistics
    {
        private readonly ConcurrentDictionary<int, int> data;


        public Statistics()
        {
            data = new ConcurrentDictionary<int, int>();
        }

        public DateTime? Start { get; set; }

        public void AddBatch(string[] logEvents)
        {
            if (Start == null)
            {
                Start = DateTime.Now;
            }

            foreach (var logEvent in logEvents)
            {
                var size = UTF8Encoding.UTF8.GetByteCount(logEvent);
                data.AddOrUpdate(size, 1, (key, oldValue) => oldValue + 1);
            }
        }
    }
}
