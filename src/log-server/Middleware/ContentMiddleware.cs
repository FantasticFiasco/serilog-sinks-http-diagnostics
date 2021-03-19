using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LogServer.Middleware
{
    public class ContentMiddleware
    {
        public const string ContentType = "Content-Type";
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLength = "Content-Length";

        private readonly RequestDelegate _next;

        public ContentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Items[ContentType] = context.Request.Headers[ContentType];
            context.Items[ContentEncoding] = context.Request.Headers[ContentEncoding];
            context.Items[ContentLength] = context.Request.Headers[ContentLength];

            return _next(context);
        }
    }
}
