using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;

namespace streamflix.extractors.Extractors
{
    public class DoodLaExtractor : Extractor
    {
        public override string Name => "DoodStream";
        public override string MainUrl => "https://dood.la";

        public override List<string> AliasUrls => new()
        {
            "https://dsvplay.com",
            "https://mikaylaarealike.com",
            "https://myvidplay.com",
            "https://playmogo.com",
            "https://do7go.com",
            "https://d000d.com"
        };

        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private readonly HttpClientService _http;
        private readonly ILogger<DoodLaExtractor> _logger;

        public DoodLaExtractor(HttpClientService http, ILogger<DoodLaExtractor> logger)
        {
            _http = http;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[DoodStream] Extracting {Link}", link);

            var embedUrl = link.Replace("/d/", "/e/");

            // Need the final URL after redirects to build md5 URL correctly
            var (document, finalUrl) = await _http.GetStringWithFinalUrlAsync(embedUrl).ConfigureAwait(false);

            // base url
            var mBase = Regex.Match(finalUrl, @"(https?://[^/]+)");
            var finalBaseUrl = mBase.Success ? mBase.Groups[1].Value : throw new Exception("Could not parse final base url");

            var md5Match = Regex.Match(document, "/pass_md5/[^']*");
            if (!md5Match.Success) throw new Exception("Could not find md5 path");
            var md5Path = md5Match.Value;
            var md5Url = finalBaseUrl + md5Path;

            // GET md5Url with referer = finalUrl
            var videoPrefix = await _http.GetStringWithHeadersAsync(md5Url, new Dictionary<string,string> { { "Referer", finalUrl } }).ConfigureAwait(false);

            var url = videoPrefix + CreateHashTable() + "?token=" + md5Url.Substring(md5Url.LastIndexOf('/') + 1);

            _logger.LogInformation("[DoodStream] Source found: {Url}", url);

            return new Video
            {
                Source = url,
                Headers = new Dictionary<string,string> { { "Referer", finalBaseUrl } }
            };
        }

        private static string CreateHashTable()
        {
            var sb = new System.Text.StringBuilder(10);
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[1];
            for (int i = 0; i < 10; i++)
            {
                rng.GetBytes(buffer);
                var idx = buffer[0] % Alphabet.Length;
                sb.Append(Alphabet[idx]);
            }
            return sb.ToString();
        }

        public class DoodLiExtractor : DoodLaExtractor
        {
            public override string MainUrl => "https://dood.li";
        }

        public class DoodExtractor : DoodLaExtractor
        {
            public override string MainUrl => "https://vide0.net";
        }
    }
}
