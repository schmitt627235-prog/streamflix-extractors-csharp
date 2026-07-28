using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace streamflix.extractors.Parsing
{
    public class AngleSharpHtmlParser
    {
        private readonly IBrowsingContext _context;

        public AngleSharpHtmlParser()
        {
            var config = Configuration.Default.WithDefaultLoader();
            _context = BrowsingContext.New(config);
        }

        public async Task<IDocument> ParseDocumentAsync(string source)
        {
            return await _context.OpenAsync(req => req.Content(source)).ConfigureAwait(false);
        }
    }
}
