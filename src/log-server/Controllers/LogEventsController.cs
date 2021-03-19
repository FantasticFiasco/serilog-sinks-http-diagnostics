using System.Linq;
using LogServer.Middleware;
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
            var contentType = (string)HttpContext.Items[ContentMiddleware.ContentType]!;
            var contentEncoding = (string)HttpContext.Items[ContentMiddleware.ContentEncoding]!;
            var contentLength = (int)HttpContext.Items[ContentMiddleware.ContentLength]!;

            var logEventSizes = Json
                .ParseArray(logEventBatch)
                .Select(ByteSize.From)
                .ToArray();

            _statistics.ReportReceivedBatch(contentType, contentEncoding, contentLength, logEventSizes);
        }
    }
}
