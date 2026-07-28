using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace streamflix.extractors
{
    public abstract class Extractor
    {
        public abstract string Name { get; }
        public abstract string MainUrl { get; }
        public virtual List<string> AliasUrls => new();
        public virtual List<Regex> RotatingDomain => new();

        // main method all subclasses must implement
        public abstract Task<Video> ExtractAsync(string link);

        // convenience helper
        public virtual Task<Video> ExtractAsync(string link, Video.Server? server = null)
        {
            return ExtractAsync(link);
        }
    }
}
