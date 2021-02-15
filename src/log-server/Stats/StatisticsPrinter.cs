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

            var below512B = statistics.EventsOfSize(LogEventSize.Below512B);
            var between512BAnd1KB = statistics.EventsOfSize(LogEventSize.Between512BAnd1KB);
            var between1And5KB = statistics.EventsOfSize(LogEventSize.Between1And5KB);
            var between5And10KB = statistics.EventsOfSize(LogEventSize.Between5And10KB);
            var between10And50KB = statistics.EventsOfSize(LogEventSize.Between10And50KB);
            var between50And100KB = statistics.EventsOfSize(LogEventSize.Between50And100KB);
            var between100And512KB = statistics.EventsOfSize(LogEventSize.Between100And512KB);
            var between512KBAnd1MB = statistics.EventsOfSize(LogEventSize.Between512KBAnd1MB);
            var between1And5MB = statistics.EventsOfSize(LogEventSize.Between1And5MB);
            var above5MB = statistics.EventsOfSize(LogEventSize.Above5MB);

            var total =
                below512B + between512BAnd1KB + between1And5KB + between5And10KB +
                between10And50KB + between50And100KB + between100And512KB + between512KBAnd1MB +
                between1And5MB + above5MB;

            messageBuilder.AppendLine("Distribution:");
            messageBuilder.AppendLine("             size < 512B  {0,9} |{1}".Format(below512B, new string('#', 10 * below512B / total)));
            messageBuilder.AppendLine("    512B  <= size < 1KB   {0,9} |{1}".Format(between512BAnd1KB, new string('#', 10 * between512BAnd1KB / total)));
            messageBuilder.AppendLine("    1KB   <= size < 5KB   {0,9} |{1}".Format(between1And5KB, new string('#', 10 * between1And5KB / total)));
            messageBuilder.AppendLine("    5KB   <= size < 10KB  {0,9} |{1}".Format(between5And10KB, new string('#', 10 * between5And10KB / total)));
            messageBuilder.AppendLine("    10KB  <= size < 50KB  {0,9} |{1}".Format(between10And50KB, new string('#', 10 * between10And50KB / total)));
            messageBuilder.AppendLine("    50KB  <= size < 100KB {0,9} |{1}".Format(between50And100KB, new string('#', 10 * between50And100KB / total)));
            messageBuilder.AppendLine("    100KB <= size < 512KB {0,9} |{1}".Format(between100And512KB, new string('#', 10 * between100And512KB / total)));
            messageBuilder.AppendLine("    512KB <= size < 1MB   {0,9} |{1}".Format(between512KBAnd1MB, new string('#', 10 * between512KBAnd1MB / total)));
            messageBuilder.AppendLine("    1MB   <= size < 5MB   {0,9} |{1}".Format(between1And5MB, new string('#', 10 * between1And5MB / total)));
            messageBuilder.AppendLine("    5MB   <= size         {0,9} |{1}".Format(above5MB, new string('#', 10 * above5MB / total)));

            logger.LogInformation(messageBuilder.ToString());
        }
    }
}
