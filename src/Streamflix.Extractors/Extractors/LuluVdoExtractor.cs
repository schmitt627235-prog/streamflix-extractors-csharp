using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class LuluVdoExtractor : Extractor
    {
        public override string Name => "LuluVdo";
        public override string MainUrl => "luluvdo.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<LuluVdoExtractor> _logger;

        public LuluVdoExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<LuluVdoExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[LuluVdo] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var source = doc.QuerySelector("video source[src]");
            if (source != null) return new Video { Source = source.GetAttribute("src") };

            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("LuluVdo: no source found");
        }
    }
}
