using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace App.HttpClients
{
    public class GzipHttpClient : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public GzipHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public void Configure(IConfiguration configuration)
        {
        }

        // TODO: Stream content instead of holding everything in memory
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            await using var input = new MemoryStream(await content.ReadAsByteArrayAsync());

            await using var output = new MemoryStream();
            await using (var gzipStream = new GZipStream(output, CompressionLevel.Optimal))
            {
                await input.CopyToAsync(gzipStream);
            }

            await using var contentStream = new MemoryStream(output.ToArray());
            var compressedContent = new StreamContent(contentStream);
            compressedContent.Headers.Add("Content-Type", "application/json");
            compressedContent.Headers.Add("Content-Encoding", "gzip");

            var response = await _httpClient.PostAsync(requestUri, compressedContent);
            return response;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
