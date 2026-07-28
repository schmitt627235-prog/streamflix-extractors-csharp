using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class GuploadExtractor : Extractor
    {
        public override string Name => "Gupload";
        public override string MainUrl => "gupload.io";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<GuploadExtractor> _logger;

        public GuploadExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<GuploadExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Gupload] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null) return new Video { Source = iframe.GetAttribute("src") };

            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("Gupload: no source found");
        }
    }
}
