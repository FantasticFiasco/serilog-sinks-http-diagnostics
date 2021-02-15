using CommandLine;

namespace App
{
    public class Options
    {
        [Option('n', "numbers", Default = 1000, HelpText = "The number of log events to write using Serilog")]
        public int Numbers { get; set; }

        [Option('d', "destination", Default = "http://localhost:8080", HelpText = "The URL of the log server")]
        public string? Destination { get; set; }
    }
}
