using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Parsing;

namespace streamflix.extractors.Extractors
{
    public class MailRuExtractor : Extractor
    {
        public override string Name => "MailRu";
        public override string MainUrl => "my.mail.ru";

        private readonly HttpClientService _http;
        private readonly AngleSharpHtmlParser _parser;
        private readonly ILogger<MailRuExtractor> _logger;

        public MailRuExtractor(HttpClientService http, AngleSharpHtmlParser parser, ILogger<MailRuExtractor> logger)
        {
            _http = http;
            _parser = parser;
            _logger = logger;
        }

        public override async Task<Video> ExtractAsync(string link)
        {
            _logger.LogInformation("[MailRu] Extracting {Link}", link);
            var html = await _http.GetStringAsync(link).ConfigureAwait(false);
            var doc = await _parser.ParseDocumentAsync(html).ConfigureAwait(false);

            // Mail.ru often exposes JSON in a script
            var scripts = doc.QuerySelectorAll("script");
            foreach (var s in scripts)
            {
                if (s.TextContent != null && s.TextContent.Contains("file"))
                {
                    var m = System.Text.RegularExpressions.Regex.Match(s.TextContent, @"(https?:\\/\\/[^"]+\\.(mp4|m3u8))");
                    if (m.Success) return new Video { Source = m.Value };
                }
            }

            throw new System.Exception("MailRu: no source found");
        }
    }
}
