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
            logger = new LoggerConfiguration()
                .WriteTo.Sink(statistics)
                .WriteTo.Http(
                    requestUri: options.Destination,
                    batchFormatter: new ArrayBatchFormatter(null))
                .CreateLogger();
        }

        private async Task RunAsync()
        {
            Log.Info("Start options");
            Log.Info($"    Destination         {options.Destination}");
            Log.Info($"    Concurrency         {options.Concurrency} tasks");
            Log.Info($"    Rate                {options.Rate} log events/sec");
            Log.Info($"    Max message size    {options.MaxMessageSize} KB");

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

        private Task[] RunTasksAsync(CancellationToken ct)
        {
            return Enumerable
                .Range(1, options.Concurrency)
                .Select(id => RunTaskAsync(id, ct))
                .ToArray();
        }

        private async Task RunTaskAsync(int id, CancellationToken ct)
        {
            var random = new Random(id);

            var taskRate = (double)options.Rate / options.Concurrency;
            var delayInMs = (int)Math.Round(1000 / taskRate);

            // Do an initial randomized delay, preventing all tasks from writing log events
            // at the exact same time
            var initialDelay = random.Next(0, delayInMs);
            await Delay(initialDelay, ct);

            while (!ct.IsCancellationRequested)
            {
                var size = random.Next(1, options.MaxMessageSize * (int)ByteSize.KB + 1);
                var message = new string('*', size);

                logger.Information(message);

                await Delay(delayInMs, ct);
            }
        }

        private async Task Delay(int delayInMs, CancellationToken ct)
        {
            try
            {
                await Task.Delay(delayInMs, ct);
            }
            catch (TaskCanceledException)
            {
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
