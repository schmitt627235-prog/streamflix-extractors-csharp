using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class AmazonDriveExtractor : Extractor
    {
        public override string Name => "AmazonDrive";
        public override string MainUrl => "amazonaws.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<AmazonDriveExtractor> _logger;

        public AmazonDriveExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<AmazonDriveExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[AmazonDrive] Extracting {Link}", link);
            // Fixed, safe regex string (normal string with escaped quotes/backslashes)
            var m = System.Text.RegularExpressions.Regex.Match(link, "https?://[^\\s\\\"']+\\.(mp4|m3u8)");
            if (m.Success) return new Video { Source = m.Value };

            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var script = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);
            var src = script.QuerySelector("source[src]");
            if (src != null) return new Video { Source = src.GetAttribute("src") };

            throw new System.Exception("AmazonDrive: no source found");
        }
    }
}
