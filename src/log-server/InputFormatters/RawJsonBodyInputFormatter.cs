using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace LogServer.InputFormatters
{
    public class RawJsonBodyInputFormatter : InputFormatter
    {
        public RawJsonBodyInputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            using var reader = new StreamReader(context.HttpContext.Request.Body);
            var content = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(content);
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }
    }
}
