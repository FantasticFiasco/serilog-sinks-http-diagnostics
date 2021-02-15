﻿using System;
using System.Linq;
using System.Threading.Tasks;
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
                .WriteTo.Http(
                    requestUri: options.Destination,
                    textFormatter: new LogEventFormatter(),
                    batchFormatter: new ArrayBatchFormatter(null))
                .CreateLogger();
        }

        private async Task RunAsync()
        {
            var tasks = StartTasks();
            await Task.WhenAll(tasks);
        }

        private void OnError(string message)
        {
            if (message.Length > 200)
            {
                message = $"{message.Substring(0, 200)}...";
            }

            Console.Error.WriteLine(message);
        }

        private Task[] StartTasks()
        {
            return Enumerable
                .Range(0, options.Concurrency)
                .Select(id => StartTask(id))
                .ToArray();
        }

        private async Task StartTask(int id)
        {
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
