using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Report;
using CommandLine;
using Serilog;
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
                    textFormatter: new LogEventFormatter(),
                    batchFormatter: new ArrayBatchFormatter(null))
                .CreateLogger();
        }

        private async Task RunAsync()
        {
            Log.Info("Options");
            Log.Info($"  Destination: {options.Destination}");
            Log.Info($"  Concurrency: {options.Concurrency} tasks");
            Log.Info($"  Rate:        {options.Rate} events/sec/task");
            Log.Info($"  Max size:    {options.MaxSize} KB");

            Serilog.Debugging.SelfLog.Enable(OnError);

            var printer = new Printer(statistics);
            printer.Start();

            var cts = new CancellationTokenSource();
            var tasks = StartTasks(cts.Token);

            Console.WriteLine("Press any key to cancel...");
            Console.ReadKey();

            cts.Cancel();
            await Task.WhenAll(tasks);
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
            var sleep = 1000 / options.Rate;

            while (!token.IsCancellationRequested)
            {
                var size = (int)(options.MaxSize * random.NextDouble());
                var message = new string('*', size);
                logger.Information(message);

                await Task.Delay(sleep);
            }
        }
    }
}
