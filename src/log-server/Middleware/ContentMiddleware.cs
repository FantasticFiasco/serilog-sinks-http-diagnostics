using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LogServer.Middleware
{
    public class ContentMiddleware
    {
        public const string ContentLength = "Content-Length";
        public const string ContentEncoding = "Content-Encoding";

        private readonly RequestDelegate _next;

        public ContentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Items[ContentLength] = context.Request.Headers[ContentLength];
            context.Items[ContentEncoding] = context.Request.Headers[ContentEncoding];

            return _next(context);
        }
    }
}
