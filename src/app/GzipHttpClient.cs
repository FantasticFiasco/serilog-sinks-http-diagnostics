using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace App
{
    public class GzipHttpClient : IHttpClient
    {
        private readonly HttpClient httpClient;

        public GzipHttpClient()
        {
            httpClient = new HttpClient();
        }

        public void Configure(IConfiguration configuration)
        {
        }

        // TODO: Stream content instead of holding everything in memory
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var contentPayload = await content.ReadAsByteArrayAsync();

            await using var compressedContentPayload = new MemoryStream();
            await using var compressedStream = new GZipStream(compressedContentPayload, CompressionMode.Compress);
            await compressedStream.WriteAsync(contentPayload, 0, contentPayload.Length);

            var compressedContent = new ByteArrayContent(compressedContentPayload.ToArray());

            var response = await httpClient.PostAsync(requestUri, compressedContent);
            return response;
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
