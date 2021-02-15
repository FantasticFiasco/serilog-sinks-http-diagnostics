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
            messageBuilder.AppendLine(string.Format("Start:        {0:O}", statistics.Start));
            messageBuilder.AppendLine(string.Format("Duration:     {0}", statistics.Start != null ? now.Subtract((DateTime)statistics.Start) : ""));
            messageBuilder.AppendLine(string.Format("Batches:      {0}", statistics.BatchCount));
            messageBuilder.AppendLine(string.Format("    /minute:  {0:N2}", statistics.BatchesPerMinute));
            messageBuilder.AppendLine(string.Format("Events:       {0}", statistics.EventCount));
            messageBuilder.AppendLine(string.Format("    /minute:  {0:N2}", statistics.EventsPerMinute));
            messageBuilder.AppendLine("Distribution:");
            messageBuilder.AppendLine(string.Format("            size < 512B  : {0}", statistics.EventsOfSize(LogEventSize.Below512B)));
            messageBuilder.AppendLine(string.Format("    512B  <= size < 1KB  : {0}", statistics.EventsOfSize(LogEventSize.Between512BAnd1KB)));
            messageBuilder.AppendLine(string.Format("    1KB   <= size < 5KB  : {0}", statistics.EventsOfSize(LogEventSize.Between1And5KB)));
            messageBuilder.AppendLine(string.Format("    5KB   <= size < 10KB : {0}", statistics.EventsOfSize(LogEventSize.Between5And10KB)));
            messageBuilder.AppendLine(string.Format("    10KB  <= size < 50KB : {0}", statistics.EventsOfSize(LogEventSize.Between10And50KB)));
            messageBuilder.AppendLine(string.Format("    50KB  <= size < 100KB: {0}", statistics.EventsOfSize(LogEventSize.Between50And100KB)));
            messageBuilder.AppendLine(string.Format("    100KB <= size < 512KB: {0}", statistics.EventsOfSize(LogEventSize.Between100And512KB)));
            messageBuilder.AppendLine(string.Format("    512KB <= size < 1MB  : {0}", statistics.EventsOfSize(LogEventSize.Between512KBAnd1MB)));
            messageBuilder.AppendLine(string.Format("    1MB   <= size < 5MB  : {0}", statistics.EventsOfSize(LogEventSize.Between1And5MB)));
            messageBuilder.AppendLine(string.Format("    5MB   <= size        : {0}", statistics.EventsOfSize(LogEventSize.Above5MB)));

            logger.LogInformation(messageBuilder.ToString());
        }
    }
}
