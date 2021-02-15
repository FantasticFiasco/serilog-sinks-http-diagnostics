using System;
using CommandLine;
using Serilog;
using Serilog.Sinks.Http.BatchFormatters;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
               .WithParsed(options => Run(options));
        }

        private static void Run(Options options)
        {
            Serilog.Debugging.SelfLog.Enable(OnError);

            ILogger log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Http(
                    requestUri: options.Destination,
                    textFormatter: new LogEventFormatter(),
                    batchFormatter: new ArrayBatchFormatter(null))
                .CreateLogger();

            for (var i = 0; i < options.Numbers; i++)
            {
                log.Information("Logging from app");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void OnError(string message)
        {
            if (message.Length > 200)
            {
                message = $"{message.Substring(0, 200)}...";
            }

            Console.Error.WriteLine(message);
        }
    }
}
