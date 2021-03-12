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
            Log.Info("App was started with:");
            Log.Info($"  Destination:       {options.Destination}");
            Log.Info($"  Concurrency:       {options.Concurrency} tasks");
            Log.Info($"  Rate:              {options.Rate} log events/sec/task");
            Log.Info($"  Max message size:  {options.MaxMessageSize} KB");

            var serilogErrors = new SerilogErrors();
            serilogErrors.Clear();
            Serilog.Debugging.SelfLog.Enable(message => serilogErrors.Add(message));

            var appState = AppState.None;

            var printer = new Printer(statistics, serilogErrors, () => appState);
            printer.Start();

            CancellationTokenSource? cts = null;
            Task[]? tasks = null;

            while (appState != AppState.Aborting)
            {
                appState = NextAppState(appState);

                // Let's do a line feed here since we might have gotten an input
                Log.Info("");

                switch (appState)
                {
                    case AppState.Running:
                        cts = new CancellationTokenSource();
                        tasks = RunTasksAsync(cts.Token);
                        break;

                    case AppState.Paused:
                    case AppState.Aborting:
                        cts?.Cancel();
                        await Task.WhenAll(tasks ?? Enumerable.Empty<Task>());
                        printer.Print();
                        break;
                }
            }
        }

        private Task[] RunTasksAsync(CancellationToken token)
        {
            return Enumerable
                .Range(1, options.Concurrency)
                .Select(id => RunTaskAsync(id, token))
                .ToArray();
        }

        private async Task RunTaskAsync(int id, CancellationToken token)
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
            switch (current)
            {
                case AppState.None:
                    return AppState.Running;

                case AppState.Running:
                case AppState.Paused:
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Spacebar:
                            return current == AppState.Running ? AppState.Paused : AppState.Running;
                        case ConsoleKey.Q:
                            return AppState.Aborting;
                    }
                    break;

                case AppState.Aborting:
                    return AppState.Aborting;
            }

            return NextAppState(current);
        }
    }
}
