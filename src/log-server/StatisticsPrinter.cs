using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogServer
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
            var now = DateTime.Now;

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("Start:      {0:O}{1}", statistics.Start, Environment.NewLine);
            messageBuilder.AppendFormat("Duration:   {0}{1}", statistics.Start != null ? now.Subtract((DateTime)statistics.Start) : "", Environment.NewLine);
            messageBuilder.AppendFormat("Batches:    {0}{1}", statistics.BatchCount, Environment.NewLine);
            messageBuilder.AppendFormat("  /minute:  {0:N2}{1}", statistics.BatchesPerMinute, Environment.NewLine);
            messageBuilder.AppendFormat("Events:     {0}{1}", statistics.EventCount, Environment.NewLine);
            messageBuilder.AppendFormat("  /minute:  {0:N2}{1}", statistics.EventsPerMinute, Environment.NewLine);

            logger.LogInformation(messageBuilder.ToString());
        }
    }
}
