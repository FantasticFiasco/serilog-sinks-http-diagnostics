using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogServer.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogServer.Controllers
{
    [ApiController]
    [Route("")]
    public class LogEventsController : ControllerBase
    {
        private readonly Statistics statistics;
        private readonly ILogger<LogEventsController> logger;

        public LogEventsController(Statistics statistics, ILogger<LogEventsController> logger)
        {
            this.statistics = statistics;
            this.logger = logger;
        }

        [HttpPost]
        public void Post([FromBody] string logEventBatch)
        {
            int batchSize = UTF8Encoding.UTF8.GetByteCount(logEventBatch);

            var logEvents = ParseLogEvents(logEventBatch);
            var logEventSizes = logEvents
                .Select(logEvent => UTF8Encoding.UTF8.GetByteCount(logEvent))
                .ToArray();

            logger.LogInformation($"Received batch of size {batchSize / ByteSize.KB} KB with {logEvents.Length} log events");
            statistics.ReportReceivedBatch(batchSize, logEventSizes);
        }

        private string[] ParseLogEvents(string batch)
        {
            batch = batch
                .TrimStart('[')
                .TrimEnd(']');

            var logEvents = new List<string>();

            var startIndex = 0;
            var squigglyBracketCounter = 0;

            for (var i = 0; i < batch.Length; i++)
            {
                if (batch[i] == '{')
                {
                    squigglyBracketCounter++;
                }
                else if (batch[i] == '}')
                {
                    squigglyBracketCounter--;
                }

                if (squigglyBracketCounter == 0 && i > startIndex)
                {
                    var logEventLength = i - startIndex + 1;

                    var logEvent = batch
                        .Substring(startIndex, logEventLength)
                        .Trim(',');

                    logEvents.Add(logEvent);

                    startIndex += logEventLength;
                }
            }

            return logEvents.ToArray();
        }
    }
}
