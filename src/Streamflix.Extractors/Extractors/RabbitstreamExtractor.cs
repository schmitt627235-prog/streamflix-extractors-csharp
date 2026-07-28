using System.Threading.Tasks;
using AngleSharp.Dom;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class RabbitstreamExtractor : Extractor
    {
        public override string Name => "Rabbitstream";
        public override string MainUrl => "rabbitstream.net";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<RabbitstreamExtractor> _logger;

        public RabbitstreamExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<RabbitstreamExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Rabbitstream] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            // look for iframe
            var iframe = doc.QuerySelector("iframe");
            if (iframe != null)
            {
                var src = iframe.GetAttribute("src");
                if (!string.IsNullOrEmpty(src))
                    return new Video { Source = src };
            }

            // look for video source tags
            var source = doc.QuerySelector("video source[src]");
            if (source != null)
            {
                var src = source.GetAttribute("src");
                return new Video { Source = src };
            }

            // regex fallback for m3u8 or mp4
            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("Rabbitstream: no source found");
        }
    }
}
