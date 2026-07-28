using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using streamflix.extractors.Http;
using streamflix.extractors.Extractors;
using streamflix.extractors.Js;

namespace streamflix.extractors.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStreamflixExtractors(this IServiceCollection services)
        {
            services.AddHttpClient<HttpClientService>();
            services.AddSingleton<Parsing.AngleSharpHtmlParser>();
            services.AddSingleton<JsEngineService>();

            // Register concrete extractors for batch-1..batch-5
            services.AddSingleton<Extractor, FilemoonExtractor>();
            services.AddSingleton<Extractor, RabbitstreamExtractor>();
            services.AddSingleton<Extractor, UpzoneExtractor>();
            services.AddSingleton<Extractor, StreamhubExtractor>();
            services.AddSingleton<Extractor, VtubeExtractor>();
            services.AddSingleton<Extractor, NuuploadExtractor>();
            services.AddSingleton<Extractor, VoeExtractor>();
            services.AddSingleton<Extractor, StreamtapeExtractor>();
            services.AddSingleton<Extractor, VidozaExtractor>();
            services.AddSingleton<Extractor, DoodLaExtractor>();
            services.AddSingleton<Extractor, TwoEmbedExtractor>();
            services.AddSingleton<Extractor, ChillxExtractor>();
            services.AddSingleton<Extractor, MoviesapiExtractor>();
            services.AddSingleton<Extractor, CloseloadExtractor>();
            services.AddSingleton<Extractor, LuluVdoExtractor>();
            services.AddSingleton<Extractor, VidPlyExtractor>();
            services.AddSingleton<Extractor, MixDropExtractor>();
            services.AddSingleton<Extractor, MailRuExtractor>();
            services.AddSingleton<Extractor, GoogleDriveExtractor>();
            services.AddSingleton<Extractor, AmazonDriveExtractor>();
            services.AddSingleton<Extractor, GuploadExtractor>();
            services.AddSingleton<Extractor, StreamSBExtractor>();
            services.AddSingleton<Extractor, StreamlareExtractor>();
            services.AddSingleton<Extractor, Mp4UploadExtractor>();

            services.AddSingleton<ExtractorResolver>(sp => new ExtractorResolver(
                sp.GetRequiredService<ILogger<ExtractorResolver>>(),
                sp.GetServices<Extractor>()));

            return services;
        }
    }
}
