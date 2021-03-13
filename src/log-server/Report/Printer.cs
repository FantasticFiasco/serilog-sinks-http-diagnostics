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
        private readonly Statistics statistics;
        private readonly Clock clock;
        private readonly ILogger<Printer> logger;

        private Timer? timer;

        public Printer(Statistics statistics, Clock clock, ILogger<Printer> logger)
        {
            this.statistics = statistics;
            this.clock = clock;
            this.logger = logger;
        }

        public override void Dispose()
        {
            timer?.Dispose();
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken ct)
        {
            timer = new Timer(OnTick, ct, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void OnTick(object? state)
        {
            if (state != null && ((CancellationToken)state).IsCancellationRequested)
            {
                return;
            }

            if (statistics.Start == null)
            {
                logger.LogInformation("Waiting for log events...");
                return;
            }

            var now = clock.Now;

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Start:                       {0:O}".Format(statistics.Start));
            messageBuilder.AppendLine("Duration:                    {0}".Format(now.Subtract(statistics.Start ?? now)));
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("Batches");
            messageBuilder.AppendLine("    count:                   {0}".Format(statistics.BatchSize.Count));
            messageBuilder.AppendLine("    per minute:              {0:N2}".Format(statistics.BatchesPerMinute));
            messageBuilder.AppendLine("    size min/avg/max:        {0} / {1} / {2}".Format(
                ByteSize.FriendlyValue(statistics.BatchSize.Min),
                ByteSize.FriendlyValue(statistics.BatchSize.Average),
                ByteSize.FriendlyValue(statistics.BatchSize.Max)));
            
            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("Log events");
            messageBuilder.AppendLine("    count:                   {0}".Format(statistics.LogEventSize.Count));
            messageBuilder.AppendLine("    per minute:              {0:N2}".Format(statistics.LogEventsPerMinute));
            messageBuilder.AppendLine("    size min/avg/max:        {0} / {1} / {2}".Format(
                ByteSize.FriendlyValue(statistics.LogEventSize.Min),
                ByteSize.FriendlyValue(statistics.LogEventSize.Average),
                ByteSize.FriendlyValue(statistics.LogEventSize.Max)));

            var rows = new[]
            {
                new DistributionRow("              size < 512 B  {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Below512B)),
                new DistributionRow("    512 B  <= size < 1 KB   {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between512BAnd1KB)),
                new DistributionRow("    1K B   <= size < 5 KB   {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between1And5KB)),
                new DistributionRow("    5K B   <= size < 10 KB  {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between5And10KB)),
                new DistributionRow("    10K B  <= size < 50 KB  {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between10And50KB)),
                new DistributionRow("    50K B  <= size < 100 KB {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between50And100KB)),
                new DistributionRow("    100 KB <= size < 512 KB {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between100And512KB)),
                new DistributionRow("    512 KB <= size < 1 MB   {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between512KBAnd1MB)),
                new DistributionRow("    1 MB   <= size < 5 MB   {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.Between1And5MB)),
                new DistributionRow("    5 MB   <= size          {0,9} |{1}", statistics.LogEventsOfSize(LogEventSize.EqualToAndAbove5MB)),
            };

            messageBuilder.AppendLine("");
            messageBuilder.AppendLine("Distribution:");

            foreach (var (template, nbrOfLogEvents) in rows)
            {
                messageBuilder.AppendLine(template.Format(nbrOfLogEvents, new string('#', (int)Math.Round(40.0 * nbrOfLogEvents / statistics.LogEventSize.Count))));
            }
            logger.LogInformation(messageBuilder.ToString());
        }

        private record DistributionRow(string Template, int NbrOfLogEvents);
    }
}

