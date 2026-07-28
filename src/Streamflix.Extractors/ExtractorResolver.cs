using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace streamflix.extractors
{
    public class ExtractorResolver
    {
        private readonly ILogger<ExtractorResolver> _logger;
        private readonly List<Extractor> _extractors;

        public ExtractorResolver(ILogger<ExtractorResolver> logger, IEnumerable<Extractor> extractors)
        {
            _logger = logger;
            _extractors = extractors.ToList();
        }

        public async Task<Video> ExtractAsync(string link, Video.Server? server = null)
        {
            var finalLink = link;

            // Universal bridge resolution (simplified)
            if (link.Contains("mysync.mov/stream/", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    _logger.LogDebug("Attempting universal bridge resolve for {Link}", link);
                    // In full conversion we would fetch and resolve JS redirects. For scaffold we skip.
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Universal bridge error");
                }
            }

            var urlRegex = new Regex("^(https?://)?(www\\.)?", RegexOptions.IgnoreCase);
            var compareUrl = urlRegex.Replace(finalLink.ToLowerInvariant(), "");

            Extractor? found = null;

            foreach (var extractor in _extractors)
            {
                var main = urlRegex.Replace(extractor.MainUrl.ToLowerInvariant(), "");
                if (compareUrl.StartsWith(main))
                {
                    found = extractor;
                    break;
                }
                foreach (var alias in extractor.AliasUrls)
                {
                    var a = urlRegex.Replace(alias.ToLowerInvariant(), "");
                    if (compareUrl.StartsWith(a))
                    {
                        found = extractor;
                        break;
                    }
                }
                if (found != null) break;
            }

            if (found == null)
            {
                // try rotating domains
                foreach (var extractor in _extractors)
                {
                    if (extractor.RotatingDomain.Any(r => r.IsMatch(compareUrl)))
                    {
                        found = extractor;
                        break;
                    }
                }
            }

            if (found == null && server != null)
            {
                foreach (var extractor in _extractors)
                {
                    if (!string.IsNullOrEmpty(server.Name) && server.Name.Contains(extractor.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        found = extractor;
                        break;
                    }
                }
            }

            if (found != null)
            {
                _logger.LogInformation("[EXTRACTOR] -> Starting: {Name} (URL: {Url})", found.Name, finalLink);
                var video = await found.ExtractAsync(finalLink).ConfigureAwait(false);
                _logger.LogInformation("[VIDEO] -> Extracted: {Source}", video.Source);
                return video;
            }

            throw new Exception($"No extractors found for URL: {finalLink}");
        }
    }
}
