using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace App.Report
{
    public class Statistics : ILogEventSink
    {
        private long _logEventCount;

        public long LogEventCount => Interlocked.Read(ref _logEventCount);

        public void Emit(LogEvent logEvent)
        {
            Interlocked.Increment(ref _logEventCount);
        }
    }
}
