using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class UpzoneExtractor : Extractor
    {
        public override string Name => "Upzone";
        public override string MainUrl => "upzone.to";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<UpzoneExtractor> _logger;

        public UpzoneExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<UpzoneExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Upzone] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null)
            {
                var src = iframe.GetAttribute("src");
                if (!string.IsNullOrEmpty(src)) return new Video { Source = src };
            }

            var source = doc.QuerySelector("a[href*='.m3u8'], a[href*='.mp4']");
            if (source != null)
            {
                var src = source.GetAttribute("href");
                return new Video { Source = src };
            }

            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("Upzone: no source found");
        }
    }
}
