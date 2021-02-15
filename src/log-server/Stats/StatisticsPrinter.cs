using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogServer.Stats
{
    public class StatisticsPrinter : BackgroundService
    {
        private readonly Statistics statistics;
        private readonly ILogger<StatisticsPrinter> logger;

        private Timer? timer;

        public StatisticsPrinter(Statistics statistics, ILogger<StatisticsPrinter> logger)
        {
            this.statistics = statistics;
            this.logger = logger;
        }

        public override void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(OnTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private void OnTick(object? state)
        {
            if (statistics.Start == null)
            {
                logger.LogInformation("Waiting for log events...");
                return;
            }

            var now = DateTime.Now;

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Start:        {0:O}".Format(statistics.Start));
            messageBuilder.AppendLine("Duration:     {0}".Format(statistics.Start != null ? now.Subtract((DateTime)statistics.Start) : ""));
            messageBuilder.AppendLine("Batches:      {0}".Format(statistics.BatchCount));
            messageBuilder.AppendLine("    /minute:  {0:N2}".Format(statistics.BatchesPerMinute));
            messageBuilder.AppendLine("Events:       {0}".Format(statistics.EventCount));
            messageBuilder.AppendLine("    /minute:  {0:N2}".Format(statistics.EventsPerMinute));

            var rows = new[]
            {
                new DistributionRow("             size < 512B  {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Below512B)),
                new DistributionRow("    512B  <= size < 1KB   {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between512BAnd1KB)),
                new DistributionRow("    1KB   <= size < 5KB   {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between1And5KB)),
                new DistributionRow("    5KB   <= size < 10KB  {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between5And10KB)),
                new DistributionRow("    10KB  <= size < 50KB  {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between10And50KB)),
                new DistributionRow("    50KB  <= size < 100KB {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between50And100KB)),
                new DistributionRow("    100KB <= size < 512KB {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between100And512KB)),
                new DistributionRow("    512KB <= size < 1MB   {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between512KBAnd1MB)),
                new DistributionRow("    1MB   <= size < 5MB   {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Between1And5MB)),
                new DistributionRow("    5MB   <= size         {0,9} |{1}", statistics.NbrOfEvents(LogEventSize.Above5MB)),
            };

            var total = rows
                .Select(row => row.NbrOfEvents)
                .Sum();

            messageBuilder.AppendLine("Distribution:");

            foreach (var row in rows)
            {
                messageBuilder.AppendLine(row.Template.Format(row.NbrOfEvents, new string('#', 20 * row.NbrOfEvents / total)));
            }

            logger.LogInformation(messageBuilder.ToString());
        }

        record DistributionRow(string Template, int NbrOfEvents);
    }
}

