using System;
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

        private Timer timer;

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

        private void OnTick(object state)
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
            messageBuilder.AppendLine("Distribution:");
            messageBuilder.AppendLine("            size < 512B  : {0}".Format(statistics.EventsOfSize(LogEventSize.Below512B)));
            messageBuilder.AppendLine("    512B  <= size < 1KB  : {0}".Format(statistics.EventsOfSize(LogEventSize.Between512BAnd1KB)));
            messageBuilder.AppendLine("    1KB   <= size < 5KB  : {0}".Format(statistics.EventsOfSize(LogEventSize.Between1And5KB)));
            messageBuilder.AppendLine("    5KB   <= size < 10KB : {0}".Format(statistics.EventsOfSize(LogEventSize.Between5And10KB)));
            messageBuilder.AppendLine("    10KB  <= size < 50KB : {0}".Format(statistics.EventsOfSize(LogEventSize.Between10And50KB)));
            messageBuilder.AppendLine("    50KB  <= size < 100KB: {0}".Format(statistics.EventsOfSize(LogEventSize.Between50And100KB)));
            messageBuilder.AppendLine("    100KB <= size < 512KB: {0}".Format(statistics.EventsOfSize(LogEventSize.Between100And512KB)));
            messageBuilder.AppendLine("    512KB <= size < 1MB  : {0}".Format(statistics.EventsOfSize(LogEventSize.Between512KBAnd1MB)));
            messageBuilder.AppendLine("    1MB   <= size < 5MB  : {0}".Format(statistics.EventsOfSize(LogEventSize.Between1And5MB)));
            messageBuilder.AppendLine("    5MB   <= size        : {0}".Format(statistics.EventsOfSize(LogEventSize.Above5MB)));

            logger.LogInformation(messageBuilder.ToString());
        }
    }
}
