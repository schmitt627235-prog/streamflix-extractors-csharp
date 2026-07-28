using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class VtubeExtractor : Extractor
    {
        public override string Name => "Vtube";
        public override string MainUrl => "vtube.to";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<VtubeExtractor> _logger;

        public VtubeExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<VtubeExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Vtube] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null)
            {
                var src = iframe.GetAttribute("src");
                return new Video { Source = src };
            }

            var script = doc.QuerySelector("script");
            if (script != null)
            {
                var m = System.Text.RegularExpressions.Regex.Match(script.TextContent ?? string.Empty, @"(https?:\\/\\/[^"]+\\.(m3u8|mp4))");
                if (m.Success) return new Video { Source = m.Value };
            }

            throw new System.Exception("Vtube: no source found");
        }
    }
}
