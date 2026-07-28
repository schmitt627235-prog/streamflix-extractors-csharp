using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class MoviesapiExtractor : Extractor
    {
        public override string Name => "Moviesapi";
        public override string MainUrl => "moviesapi.com";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<MoviesapiExtractor> _logger;

        public MoviesapiExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<MoviesapiExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[Moviesapi] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            var iframe = doc.QuerySelector("iframe[src]");
            if (iframe != null) return new Video { Source = iframe.GetAttribute("src") };

            var anchor = doc.QuerySelector("a[href*='.m3u8'], a[href*='.mp4']");
            if (anchor != null) return new Video { Source = anchor.GetAttribute("href") };

            throw new System.Exception("Moviesapi: no source found");
        }
    }
}
