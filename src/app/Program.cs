using System;
using System.Linq;
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

            random = new Random();

            Serilog.Debugging.SelfLog.Enable(OnError);

            logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink<Statistics>()
                .WriteTo.Http(
                    requestUri: options.Destination,
                    textFormatter: new LogEventFormatter(),
                    batchFormatter: new ArrayBatchFormatter(null))
                .CreateLogger();
        }

        private async Task RunAsync()
        {
            var statistics = new Statistics();

            var printer = new Printer(statistics);
            printer.Start();

            var tasks = StartTasks();
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

        private Task[] StartTasks()
        {
            return Enumerable
                .Range(1, options.Concurrency)
                .Select(id => StartTask(id))
                .ToArray();
        }

        private async Task StartTask(int id)
        {
            Log.Info($"Starting up task {id}...");

            var sleep = 1000 / options.Rate;

            while (true)
            {
                var size = (int)(options.MaxSize * random.NextDouble());
                var message = new string('*', size);
                logger.Information(message);

                await Task.Delay(sleep);
            }
        }
    }
}
