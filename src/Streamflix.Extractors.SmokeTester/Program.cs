using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using streamflix.extractors;

namespace Streamflix.Extractors.SmokeTester
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var urls = new[]
            {
                "https://matthewhotelscience.com/dm89qzbkjchm",
                "https://playmogo.com/e/3cu5pkdxzrxk",
                "https://firestream.to/e/ZbDer-Tg",
                "https://vidaraa.cc/e/uoCm09Vs1t70s",
                "https://vidsonic.net/e/ji235d9x6iin"
            };

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddSimpleConsole(options => { options.TimestampFormat = "[HH:mm:ss] "; }));
            services.AddStreamflixExtractors();
            var sp = services.BuildServiceProvider();

            var logger = sp.GetRequiredService<ILogger<Program>>();
            var resolver = sp.GetRequiredService<ExtractorResolver>();

            foreach (var url in urls)
            {
                logger.LogInformation("Testing URL: {Url}", url);
                try
                {
                    var video = await resolver.ExtractAsync(url).ConfigureAwait(false);
                    Console.WriteLine("URL: " + url);
                    Console.WriteLine("  Source: " + video.Source);
                    if (video.Headers != null)
                    {
                        Console.WriteLine("  Headers:");
                        foreach (var kv in video.Headers)
                        {
                            Console.WriteLine($"    {kv.Key}: {kv.Value}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("URL: " + url);
                    Console.WriteLine("  ERROR: " + ex.Message);
                }
                Console.WriteLine();
            }

            return 0;
        }
    }
}
