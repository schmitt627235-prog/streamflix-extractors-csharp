using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;

namespace streamflix.extractors.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStreamflixExtractors(this IServiceCollection services)
        {
            services.AddHttpClient<HttpClientService>();
            services.AddSingleton<Parsing.AngleSharpHtmlParser>();

            // Register extractors here. For now register zero or a few stubs.
            // In the full conversion step we'll add all concrete extractor implementations.

            services.AddSingleton<ExtractorResolver>(sp => new ExtractorResolver(
                sp.GetRequiredService<ILogger<ExtractorResolver>>(),
                sp.GetServices<Extractor>()));

            return services;
        }
    }
}
