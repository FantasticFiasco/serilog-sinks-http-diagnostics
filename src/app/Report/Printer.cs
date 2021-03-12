using System;
using System.Threading;

namespace App.Report
{
    public class Printer : IDisposable
    {
        private readonly Statistics statistics;
        private readonly Errors errors;
        private readonly Func<AppState> appStateProvider;

        private Timer? timer;

        public Printer(Statistics statistics, Errors errors, Func<AppState> appStateProvider)
        {
            this.statistics = statistics;
            this.errors = errors;
            this.appStateProvider = appStateProvider;
        }

        public void Start()
        {
            timer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
        }

        public void Print()
        {
            Log.Info(string.Format(MessageFormat(), statistics.LogEventCount, errors.Count));
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private void OnTick(object? state)
        {
            Print();
        }

        private string MessageFormat()
        {
            var appState = appStateProvider();
            switch (appState)
            {
                case AppState.Running:
                    return "[RUNNING]  Number of written log events: {0}; Errors: {1} (SPACE: Pause,  Q: Quit)";

                case AppState.Paused:
                    return "[PAUSED]   Number of written log events: {0}; Errors: {1} (SPACE: Resume, Q: Quit)";

                case AppState.Aborting:
                    return "[ABORTING] Number of written log events: {0}; Errors: {1}";

                default:
                    throw new Exception($"Unsupported app state: {appState}");
            }
        }
    }
}
