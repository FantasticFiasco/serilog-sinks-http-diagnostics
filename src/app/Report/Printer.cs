using System;
using System.Threading;

namespace App.Report
{
    public class Printer : IDisposable
    {
        private readonly Statistics statistics;

        private Timer? timer;

        public Printer(Statistics statistics)
        {
            this.statistics = statistics;
        }

        public void Start()
        {
            timer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private void OnTick(object? state)
        {
            Log.Info($"Number of written log events: {statistics.LogEventCount}");
        }
    }
}
