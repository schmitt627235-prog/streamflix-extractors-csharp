using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class StreamlareExtractor : Extractor
    {
        public override string Name => "Streamlare";
        public override string MainUrl => "streamlare.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<StreamlareExtractor> _logger;

        public StreamlareExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<StreamlareExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Streamlare] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var script = doc.QuerySelectorAll("script");
            foreach (var s in script)
            {
                var m = System.Text.RegularExpressions.Regex.Match(s.TextContent ?? string.Empty, @"(https?:\\/\\/[^"]+\\.(m3u8|mp4))");
                if (m.Success) return new Video { Source = m.Value };
            }

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null) return new Video { Source = iframe.GetAttribute("src") };

            throw new System.Exception("Streamlare: no source found");
        }
    }
}
