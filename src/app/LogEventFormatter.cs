using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace App
{
    public class LogEventFormatter : ITextFormatter
    {
        public void Format(LogEvent logEvent, TextWriter output)
        {
            output.Write("{\"payload\":\"");
            output.Write(logEvent.MessageTemplate);
            output.Write("\"}");
        }
    }
}
