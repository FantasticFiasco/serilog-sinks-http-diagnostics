using CommandLine;

namespace App
{
    public class Options
    {
        [Option(
            'd',
            "destination", Default = "http://localhost:8080", HelpText = "The URL of the log server")]
        public string Destination { get; set; } = "http://localhost:8080";

        [Option('c', "concurrency", Default = 10, HelpText = "The number of concurrent tasks writing log events")]
        public int Concurrency { get; set; } = 10;

        [Option('r', "rate", Default = 1, HelpText = "The number of log events per second each task is writing")]
        public int Rate { get; set; } = 1;

        [Option('m', "max-message-size", Default = 10, HelpText = "The maximum size, in KB, of a logged message. Please note that this is a limit of the message size, not the serialized log event size.")]
        public int MaxSize { get; set; } = 10;
    }
}
