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
            messageBuilder.AppendFormatted("start                                  {0:O}", _statistics.Start);
            messageBuilder.AppendFormatted("duration                               {0}", now.Subtract(_statistics.Start ?? now));
            messageBuilder.AppendNewLine();
            messageBuilder.AppendLine("batches");
            messageBuilder.AppendTabbedFormatted("content-type                       {0}", _statistics.ContentType);
            messageBuilder.AppendTabbedFormatted("content-encoding                   {0}", _statistics.ContentEncoding);
            messageBuilder.AppendTabbedFormatted("count                              {0}", _statistics.ContentLength.Count);
            messageBuilder.AppendTabbedFormatted("per second                         {0:N2}", _statistics.RequestsPerSecond);
            messageBuilder.AppendTabbedFormatted("compressed size (min/avg/max)      {0} / {1} / {2}",
                ByteSize.FriendlyValue(_statistics.ContentLength.Min),
                ByteSize.FriendlyValue(_statistics.ContentLength.Average),
                ByteSize.FriendlyValue(_statistics.ContentLength.Max));
            messageBuilder.AppendTabbedFormatted("uncompressed size (min/avg/max)    {0} / {1} / {2}",
                ByteSize.FriendlyValue(_statistics.BatchSize.Min),
                ByteSize.FriendlyValue(_statistics.BatchSize.Average),
                ByteSize.FriendlyValue(_statistics.BatchSize.Max));
            messageBuilder.AppendTabbedFormatted("compression ratio                  {0:N2}", _statistics.CompressionRatio);
            messageBuilder.AppendNewLine();
            messageBuilder.AppendLine("log events");
            messageBuilder.AppendTabbedFormatted("count                              {0}", _statistics.LogEventSize.Count);
            messageBuilder.AppendTabbedFormatted("per second                         {0:N2}", _statistics.LogEventsPerSecond);
            messageBuilder.AppendTabbedFormatted("size (min/avg/max)                 {0} / {1} / {2}",
                ByteSize.FriendlyValue(_statistics.LogEventSize.Min),
                ByteSize.FriendlyValue(_statistics.LogEventSize.Average),
                ByteSize.FriendlyValue(_statistics.LogEventSize.Max));
            messageBuilder.AppendTabbedFormatted("per batch (min/avg/max)            {0} / {1:N2} / {2}",
                _statistics.LogEventsPerBatch.Min,
                _statistics.LogEventsPerBatch.Average,
                _statistics.LogEventsPerBatch.Max);

            var rows = new[]
            {
                new DistributionRow("                                                 size < 512 B  {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Below512B)),
                new DistributionRow("                                       512 B  <= size < 1 KB   {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between512BAnd1KB)),
                new DistributionRow("                                       1K B   <= size < 5 KB   {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between1And5KB)),
                new DistributionRow("                                       5K B   <= size < 10 KB  {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between5And10KB)),
                new DistributionRow("                                       10K B  <= size < 50 KB  {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between10And50KB)),
                new DistributionRow("                                       50K B  <= size < 100 KB {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between50And100KB)),
                new DistributionRow("                                       100 KB <= size < 512 KB {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between100And512KB)),
                new DistributionRow("                                       512 KB <= size < 1 MB   {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between512KBAnd1MB)),
                new DistributionRow("                                       1 MB   <= size < 5 MB   {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.Between1And5MB)),
                new DistributionRow("                                       5 MB   <= size          {0,9} |{1}", _statistics.LogEventsOfSize(SizeBucket.EqualToAndAbove5MB)),
            };

            messageBuilder.AppendTabbedFormatted("distribution");

            foreach (var (template, nbrOfLogEvents) in rows)
            {
                messageBuilder.AppendLine(template.Format(nbrOfLogEvents, new string('#', (int)Math.Round(40.0 * nbrOfLogEvents / _statistics.LogEventSize.Count))));
            }
            _logger.LogInformation(messageBuilder.ToString());
        }

        private record DistributionRow(string Template, int NbrOfLogEvents);
    }
}
