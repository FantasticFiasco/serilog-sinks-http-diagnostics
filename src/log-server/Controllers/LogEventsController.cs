using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogServer.Controllers
{
    [ApiController]
    [Route("")]
    public class LogEventsController : ControllerBase
    {
        private readonly ILogger<LogEventsController> logger;

        public LogEventsController(ILogger<LogEventsController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public void Post([FromBody] LogEvents body)
        {
            logger.LogInformation($"Received batch of {body.Events.Length} log events");

            foreach (var logEvent in body.Events)
            {
                logger.LogInformation(logEvent.RenderedMessage);
            }
        }
    }

    public class LogEvents
    {
        public LogEvent[] Events { get; set; }
    }

    public class LogEvent
    {
        public DateTime Timestamp { get; set; }

        public string Level { get; set; }

        public string RenderedMessage { get; set; }
    }
}
