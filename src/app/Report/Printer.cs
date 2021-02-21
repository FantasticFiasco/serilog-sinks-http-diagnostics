using System;
using System.Threading;

namespace App.Report
{
    public class Printer : IDisposable
    {
        private readonly Statistics statistics;
        private readonly Func<AppState> appStateProvider;

        private Timer? timer;

        public Printer(Statistics statistics, Func<AppState> appStateProvider)
        {
            this.statistics = statistics;
            this.appStateProvider = appStateProvider;
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
            Log.Info(string.Format(MessageFormat(), statistics.LogEventCount));
        }

        private string MessageFormat()
        {
            var appState = appStateProvider();
            switch (appState)
            {
                case AppState.Running:
                    return "[RUNNING]  Number of written log events: {0} (SPACE: Pause,  Q: Quit)";

                case AppState.Paused:
                    return "[PAUSED]   Number of written log events: {0} (SPACE: Resume, Q: Quit)";

                case AppState.Aborting:
                    return "[ABORTING] Number of written log events: {0}";

                default:
                    throw new Exception($"Unsupported app state: {appState}");
            }
        }
    }
}
