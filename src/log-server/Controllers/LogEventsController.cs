using System.Linq;
using LogServer.Report;
using Microsoft.AspNetCore.Mvc;

namespace LogServer.Controllers
{
    [ApiController]
    [Route("")]
    public class LogEventsController : ControllerBase
    {
        private readonly Statistics _statistics;

        public LogEventsController(Statistics statistics)
        {
            _statistics = statistics;
        }

        [HttpPost]
        public void Post([FromBody] string logEventBatch)
        {
            int batchSize = ByteSize.From(logEventBatch);

            var logEventSizes = Json
                .ParseArray(logEventBatch)
                .Select(logEvent => ByteSize.From(logEvent))
                .ToArray();

            _statistics.ReportReceivedBatch(batchSize, logEventSizes);
        }
    }
}
