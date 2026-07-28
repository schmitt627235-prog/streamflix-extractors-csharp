using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class NuuploadExtractor : Extractor
    {
        public override string Name => "Nuupload";
        public override string MainUrl => "nuupload.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<NuuploadExtractor> _logger;

        public NuuploadExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<NuuploadExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Nuupload] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var source = doc.QuerySelector("video source[src], source[src]");
            if (source != null)
            {
                var src = source.GetAttribute("src");
                return new Video { Source = src };
            }

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null) return new Video { Source = iframe.GetAttribute("src") };

            var m = System.Text.RegularExpressions.Regex.Match(html, @"https?:\\/\\/[^"]+\\.(m3u8|mp4)");
            if (m.Success) return new Video { Source = m.Value };

            throw new System.Exception("Nuupload: no source found");
        }
    }
}
