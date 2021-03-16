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

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return httpClient.PostAsync(requestUri, content);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
