using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LogServer.Middleware
{
    public class GzipRequestMiddleware
    {
        private const string ContentEncoding = "Content-Encoding";
        private const string Gzip = "gzip";

        private readonly RequestDelegate _next;
        
        public GzipRequestMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(ContentEncoding, out var value))
            {
                if (value == Gzip)
                {
                    context.Request.Body = new GZipStream(
                        context.Request.Body,
                        CompressionMode.Decompress,
                        true);
                }
            }

            return _next(context);
        }
    }
}
