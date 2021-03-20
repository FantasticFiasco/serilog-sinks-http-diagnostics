using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LogServer
{
    public class GzipRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ContentEncodingHeader = "Content-Encoding";
        private const string ContentEncodingGzip = "gzip";
        private const string ContentEncodingDeflate = "deflate";
        private readonly ILogger<GzipRequestMiddleware> _logger;

        public GzipRequestMiddleware(RequestDelegate next, ILogger<GzipRequestMiddleware> logger)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
            this._logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["content-length"] = context.Request.ContentLength;

            if (context.Request.Headers.Keys.Contains(ContentEncodingHeader) && (context.Request.Headers[ContentEncodingHeader] == ContentEncodingGzip || context.Request.Headers[ContentEncodingHeader] == ContentEncodingDeflate))
            {
                var contentEncoding = context.Request.Headers[ContentEncodingHeader];
                var decompressor = contentEncoding == ContentEncodingGzip ? new GZipStream(context.Request.Body, CompressionMode.Decompress, true) : (Stream)new DeflateStream(context.Request.Body, CompressionMode.Decompress, true);
                context.Request.Body = decompressor;
            }
            await _next(context);
        }
    }
}
