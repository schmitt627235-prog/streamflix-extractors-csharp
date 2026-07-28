using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class Mp4UploadExtractor : Extractor
    {
        public override string Name => "Mp4Upload";
        public override string MainUrl => "mp4upload.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<Mp4UploadExtractor> _logger;

        public Mp4UploadExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<Mp4UploadExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Mp4Upload] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var scripts = doc.QuerySelectorAll("script");
            foreach (var s in scripts)
            {
                var m = System.Text.RegularExpressions.Regex.Match(s.TextContent ?? string.Empty, @"(https?:\\/\\/[^"]+\\.(mp4|m3u8))");
                if (m.Success) return new Video { Source = m.Value };
            }

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null) return new Video { Source = iframe.GetAttribute("src") };

            throw new System.Exception("Mp4Upload: no source found");
        }
    }
}
