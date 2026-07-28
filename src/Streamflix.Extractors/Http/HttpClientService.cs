using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace streamflix.extractors.Http
{
    public class HttpClientService
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpClientService> _logger;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

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

        public async Task<(string content, string finalUrl)> GetStringWithFinalUrlAsync(string url, IDictionary<string,string>? headers = null)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            if (headers != null)
            {
                foreach (var kv in headers)
                    req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }

            _logger.LogDebug("GET (with finalUrl) {Url}", url);
            var resp = await _client.SendAsync(req).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            var final = resp.RequestMessage?.RequestUri?.ToString() ?? url;
            return (content, final);
        }

        public async Task<string> GetStringWithHeadersAsync(string url, IDictionary<string,string>? headers = null)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            if (headers != null)
            {
                foreach (var kv in headers)
                    req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }

            _logger.LogDebug("GET {Url} with headers", url);
            var resp = await _client.SendAsync(req).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<T?> GetJsonAsync<T>(string url)
        {
            var s = await GetStringAsync(url).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(s, _jsonOptions);
        }

        public async Task<T?> PostJsonAsync<T>(string url, object payload, IDictionary<string, string>? headers = null)
        {
            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            using var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    req.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
                }
            }

            _logger.LogDebug("POST {Url} with payload length {Len}", url, json.Length);
            var resp = await _client.SendAsync(req).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var s = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(s, _jsonOptions);
        }
    }
}
