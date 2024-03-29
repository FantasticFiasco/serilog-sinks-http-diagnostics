using CommandLine;

namespace App
{
    public class Options
    {
        [Option("destination", Default = "http://localhost:8080", HelpText = "The URL of the log server")]
        public string Destination { get; set; } = "http://localhost:8080";

        [Option("concurrency", Default = 10, HelpText = "The number of concurrent tasks writing log events")]
        public int Concurrency { get; set; } = 10;

        [Option("rate", Default = 10, HelpText = "The number of log events per second")]
        public int Rate { get; set; } = 1;

        [Option("max-message-size", Default = 10, HelpText = "The maximum size, in KB, of a logged message. Please note that this is a limit of the message size, not the serialized log event size.")]
        public int MaxMessageSize { get; set; } = 10;

        [Option("compression", Default = Compression.None, HelpText = "The request compression method [None, Gzip]")]
        public Compression Compression { get; set; } = Compression.None;
    }

    public enum Compression
    {
        None,
        Gzip
    }
}
