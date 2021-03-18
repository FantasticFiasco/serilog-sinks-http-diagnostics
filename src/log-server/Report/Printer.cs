using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogServer.Time;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogServer.Report
{
    public class Printer : BackgroundService
    {
        private readonly Statistics _statistics;
        private readonly Clock _clock;
        private readonly ILogger<Printer> _logger;

        private Timer? _timer;

        public Printer(Statistics statistics, Clock clock, ILogger<Printer> logger)
        {
            _statistics = statistics;
            _clock = clock;
            _logger = logger;
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken ct)
        {
            _timer = new Timer(OnTick, ct, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void OnTick(object? state)
        {
            if (state != null && ((CancellationToken)state).IsCancellationRequested)
            {
                return;
            }

            if (_statistics.Start == null)
            {
                _logger.LogInformation("Waiting for log events...");
                return;
            }

            var now = _clock.Now;

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("start                          {0:O}".Format(_statistics.Start));
            messageBuilder.AppendLine("duration                       {0}".Format(now.Subtract(_statistics.Start ?? now)));
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("batches");
            messageBuilder.AppendLine("    count                      {0}".Format(_statistics.BatchSize.Count));
            messageBuilder.AppendLine("    per second                 {0:N2}".Format(_statistics.BatchesPerSecond));
            messageBuilder.AppendLine("    size (min/avg/max)         {0} / {1} / {2}".Format(
                ByteSize.FriendlyValue(_statistics.BatchSize.Min),
                ByteSize.FriendlyValue(_statistics.BatchSize.Average),
                ByteSize.FriendlyValue(_statistics.BatchSize.Max)));
            
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("log events");
            messageBuilder.AppendLine("    count                      {0}".Format(_statistics.LogEventSize.Count));
            messageBuilder.AppendLine("    per second                 {0:N2}".Format(_statistics.LogEventsPerSecond));
            messageBuilder.AppendLine("    size (min/avg/max)         {0} / {1} / {2}".Format(
                ByteSize.FriendlyValue(_statistics.LogEventSize.Min),
                ByteSize.FriendlyValue(_statistics.LogEventSize.Average),
                ByteSize.FriendlyValue(_statistics.LogEventSize.Max)));
            messageBuilder.AppendLine("    per batch (min/avg/max)    {0} / {1:N2} / {2}".Format(
                _statistics.LogEventsPerBatch.Min,
                _statistics.LogEventsPerBatch.Average,
                _statistics.LogEventsPerBatch.Max));

            var rows = new[]
            {
                new DistributionRow("              size < 512 B  {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Below512B)),
                new DistributionRow("    512 B  <= size < 1 KB   {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between512BAnd1Kb)),
                new DistributionRow("    1K B   <= size < 5 KB   {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between1And5Kb)),
                new DistributionRow("    5K B   <= size < 10 KB  {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between5And10Kb)),
                new DistributionRow("    10K B  <= size < 50 KB  {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between10And50Kb)),
                new DistributionRow("    50K B  <= size < 100 KB {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between50And100Kb)),
                new DistributionRow("    100 KB <= size < 512 KB {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between100And512Kb)),
                new DistributionRow("    512 KB <= size < 1 MB   {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between512KbAnd1Mb)),
                new DistributionRow("    1 MB   <= size < 5 MB   {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.Between1And5Mb)),
                new DistributionRow("    5 MB   <= size          {0,9} |{1}", _statistics.LogEventsOfSize(LogEventSize.EqualToAndAbove5Mb)),
            };

            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("distribution");

            foreach (var (template, nbrOfLogEvents) in rows)
            {
                messageBuilder.AppendLine(template.Format(nbrOfLogEvents, new string('#', (int)Math.Round(40.0 * nbrOfLogEvents / _statistics.LogEventSize.Count))));
            }
            _logger.LogInformation(messageBuilder.ToString());
        }

        private record DistributionRow(string Template, int NbrOfLogEvents);
    }
}

