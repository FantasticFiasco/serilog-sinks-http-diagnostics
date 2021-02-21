using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Report;
using CommandLine;
using Serilog;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.BatchFormatters;

namespace App
{
    class Program
    {
        private readonly Options options;
        private readonly Statistics statistics;
        private readonly Random random;
        private readonly ILogger logger;

        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
               .WithParsedAsync(options => new Program(options).RunAsync());
        }

        private Program(Options options)
        {
            this.options = options;

            statistics = new Statistics();
            random = new Random();
            logger = new LoggerConfiguration()
                .WriteTo.Sink(statistics)
                .WriteTo.Http(
                    requestUri: options.Destination,
                    batchFormatter: new ArrayBatchFormatter(null))
                .CreateLogger();
        }

        private async Task RunAsync()
        {
            Log.Info("App started with the following options:");
            Log.Info($"  Destination:       {options.Destination}");
            Log.Info($"  Concurrency:       {options.Concurrency} tasks");
            Log.Info($"  Rate:              {options.Rate} log events/sec/task");
            Log.Info($"  Max message size:  {options.MaxMessageSize} KB");
            Log.Info("");

            Serilog.Debugging.SelfLog.Enable(OnError);

            var appState = AppState.Running;
            var printer = new Printer(statistics, () => appState);
            printer.Start();

            while (appState != AppState.Aborting)
            {
                var cts = new CancellationTokenSource();
                Task[] tasks = new Task[0];

                if (appState == AppState.Running)
                {
                    tasks = StartTasks(cts.Token);
                }

                appState = NextAppState(appState);

                cts.Cancel();
                await Task.WhenAll(tasks);
            }
        }

        private void OnError(string message)
        {
            if (message.Length > 200)
            {
                message = $"{message.Substring(0, 200)}...";
            }

            Log.Error($"[DIAGNOSTICS] {message}");
        }

        private Task[] StartTasks(CancellationToken token)
        {
            return Enumerable
                .Range(1, options.Concurrency)
                .Select(id => StartTask(id, token))
                .ToArray();
        }

        private async Task StartTask(int id, CancellationToken token)
        {
            var delayInMs = 1000 / options.Rate;

            while (!token.IsCancellationRequested)
            {
                var size = (int)Math.Round(options.MaxMessageSize * ByteSize.KB * random.NextDouble());
                var message = new string('*', size);

                logger.Information(message);

                await Task.Delay(delayInMs);
            }
        }

        private AppState NextAppState(AppState current)
        {
            while (true)
            {
                var key = Console.ReadKey().Key;

                switch (current)
                {
                    case AppState.Running:
                        switch (key)
                        {
                            case ConsoleKey.Spacebar: return AppState.Paused;
                            case ConsoleKey.Q: return AppState.Aborting;
                        }
                        break;

                    case AppState.Paused:
                        switch (key)
                        {
                            case ConsoleKey.Spacebar: return AppState.Running;
                            case ConsoleKey.Q: return AppState.Aborting;
                        }
                        break;
                }
            }
        }
    }
}
