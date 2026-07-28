using System.Threading.Tasks;
using Jint;
using Microsoft.Extensions.Logging;

namespace streamflix.extractors.Js
{
    public class JsEngineService
    {
        private readonly ILogger<JsEngineService> _logger;
        private readonly Engine _engine;

        public JsEngineService(ILogger<JsEngineService> logger)
        {
            _logger = logger;
            _engine = new Engine(cfg => cfg.LimitRecursion(64).Strict());
        }

        public Task<string?> ExecuteAsync(string script, string expression = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    _engine.Execute(script);
                    if (!string.IsNullOrEmpty(expression))
                    {
                        var v = _engine.Evaluate(expression);
                        return v?.ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogWarning(ex, "JS execution failed");
                }
                return null;
            });
        }
    }
}
