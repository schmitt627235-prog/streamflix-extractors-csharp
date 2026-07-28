using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class VidozaExtractor : Extractor
    {
        public override string Name => "Vidoza";
        public override string MainUrl => "vidoza.net";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<VidozaExtractor> _logger;

        public VidozaExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<VidozaExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Vidoza] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var source = doc.QuerySelector("source[src]");
            if (source != null) return new Video { Source = source.GetAttribute("src") };

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null) return new Video { Source = iframe.GetAttribute("src") };

            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("Vidoza: no source found");
        }
    }
}
