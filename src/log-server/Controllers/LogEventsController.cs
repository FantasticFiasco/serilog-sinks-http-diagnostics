using System.Linq;
using System.Text;
using LogServer.Report;
using Microsoft.AspNetCore.Mvc;

namespace LogServer.Controllers
{
    [ApiController]
    [Route("")]
    public class LogEventsController : ControllerBase
    {
        private readonly Statistics statistics;

        public LogEventsController(Statistics statistics)
        {
            this.statistics = statistics;
        }

        [HttpPost]
        public void Post([FromBody] string logEventBatch)
        {
            int batchSize = UTF8Encoding.UTF8.GetByteCount(logEventBatch);

            var logEventSizes = Json
                .ParseArray(logEventBatch)
                .Select(logEvent => UTF8Encoding.UTF8.GetByteCount(logEvent))
                .ToArray();

            statistics.ReportReceivedBatch(batchSize, logEventSizes);
        }
    }
}
