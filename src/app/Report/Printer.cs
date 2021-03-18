using System;
using System.Threading;

namespace App.Report
{
    public class Printer : IDisposable
    {
        private readonly Statistics _statistics;
        private readonly SerilogErrors _serilogErrors;
        private readonly Func<AppState> _appStateProvider;

        private Timer? _timer;

        public Printer(Statistics statistics, SerilogErrors serilogErrors, Func<AppState> appStateProvider)
        {
            _statistics = statistics;
            _serilogErrors = serilogErrors;
            _appStateProvider = appStateProvider;
        }

        public void Start()
        {
            _timer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
        }

        public void Print()
        {
            Log.Info(string.Format(MessageFormat(), _statistics.LogEventCount, _serilogErrors.Count));
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void OnTick(object? state)
        {
            Print();
        }

        private string MessageFormat()
        {
            var appState = _appStateProvider();
            switch (appState)
            {
                case AppState.Running:
                    return "[RUNNING]  Number of written log events: {0}; Serilog errors: {1} (SPACE: Pause,  Q: Quit)";

                case AppState.Paused:
                    return "[PAUSED]   Number of written log events: {0}; Serilog errors: {1} (SPACE: Resume, Q: Quit)";

                case AppState.Aborting:
                    return "[ABORTING] Number of written log events: {0}; Serilog errors: {1}";

                default:
                    throw new Exception($"Unsupported app state: {appState}");
            }
        }
    }
}
