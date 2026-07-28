using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class StreamhubExtractor : Extractor
    {
        public override string Name => "Streamhub";
        public override string MainUrl => "streamhub.to";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<StreamhubExtractor> _logger;

        public StreamhubExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<StreamhubExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Streamhub] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null)
            {
                var src = iframe.GetAttribute("src");
                return new Video { Source = src };
            }

            var source = doc.QuerySelector("source[src]");
            if (source != null)
            {
                var src = source.GetAttribute("src");
                return new Video { Source = src };
            }

            // Robust regex: verbatim string with \S+ to avoid quote/escape issues
            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?://\S+\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("Streamhub: no source found");
        }
    }
}
