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
            var contentType = Request.Headers["Content-Type"].FirstOrDefault() ?? "";
            var contentEncoding = Request.Headers["Content-Encoding"].FirstOrDefault() ?? "";

            var batchSize = ByteSize.From(logEventBatch);

            var logEventSizes = Json
                .ParseArray(logEventBatch)
                .Select(ByteSize.From)
                .ToArray();

            _statistics.ReportReceivedBatch(contentType, contentEncoding, batchSize, logEventSizes);
        }
    }
}
