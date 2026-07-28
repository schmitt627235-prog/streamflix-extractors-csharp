using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace streamflix.extractors.Http
{
    public class HttpClientService
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpClientService> _logger;

        public HttpClientService(HttpClient client, ILogger<HttpClientService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> GetStringAsync(string url)
        {
            _logger.LogDebug("GET {Url}", url);
            var resp = await _client.GetAsync(url).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
