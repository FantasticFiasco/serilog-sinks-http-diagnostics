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
            ILogger log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Http(
                    requestUri: options.Destination,
                    textFormatter: new LogEventFormatter(),
                    batchFormatter: new ArrayBatchFormatter())
                .CreateLogger();

            for (var i = 0; i < options.Numbers; i++)
            {
                log.Information("{@Date} Logging from app", DateTime.Now);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
