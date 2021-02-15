using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace App.Report
{
    public class Statistics : ILogEventSink
    {
        private long logEventCount;

        public void Emit(LogEvent logEvent)
        {
            Interlocked.Increment(ref logEventCount);
        }
    }
}
