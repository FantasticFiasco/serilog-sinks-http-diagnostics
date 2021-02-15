using System.Linq;
using LogServer.Stats;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogServer.Controllers
{
    public record LogEvent(string Payload);

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
        public void Post([FromBody] LogEvent[] logEvents)
        {
            logger.LogInformation($"Received batch of {logEvents.Length} log events");

            statistics.AddBatch(logEvents.Select(logEvent => logEvent.Payload).ToArray());
        }
    }
}
