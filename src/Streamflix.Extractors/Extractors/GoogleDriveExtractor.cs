using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class GoogleDriveExtractor : Extractor
    {
        public override string Name => "GoogleDrive";
        public override string MainUrl => "drive.google.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<GoogleDriveExtractor> _logger;

        public GoogleDriveExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<GoogleDriveExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[GoogleDrive] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            // Look for fmt_stream_map or direct file links
            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(mp4|m3u8)");
            if (m.Success) return new Video { Source = m.Value };

            // Try query param export=download
            if (link.Contains("/file/d/"))
            {
                // convert to direct download format
                var idMatch = System.Text.RegularExpressions.Regex.Match(link, @"/file/d/([a-zA-Z0-9_-]+)");
                if (idMatch.Success)
                {
                    var id = idMatch.Groups[1].Value;
                    var direct = $"https://drive.google.com/uc?export=download&id={id}";
                    return new Video { Source = direct };
                }
            }

            throw new System.Exception("GoogleDrive: no source found");
        }
    }
}
