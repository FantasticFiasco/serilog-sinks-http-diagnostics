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

            var logEventSizes = Json
                .ParseArray(logEventBatch)
                .Select(logEvent => UTF8Encoding.UTF8.GetByteCount(logEvent))
                .ToArray();

            logger.LogInformation($"Received batch of size {batchSize / ByteSize.KB} KB with {logEventSizes.Length} log events");
            statistics.ReportReceivedBatch(batchSize, logEventSizes);
        }
    }
}
