using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using NLog;

namespace DotNetCoreWebApi.Metrics
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private static readonly Logger _metricsLog = LogManager.GetLogger("MetricsLogger");
        
        [HttpGet]
        public IActionResult GetMetrics()
        {
            _metricsLog.Info($"=====> METRICS REQUEST");
            var metrics = MetricsMiddleware.GetMetrics();
            _metricsLog.Info($"<===== COMPLETE | METRICS:\n{JsonSerializer.Serialize(metrics)}");
            return Ok(metrics);
        }

        [HttpGet("prometheus")]
        public async Task<IActionResult> GetPrometheusMetrics()
        {
            _metricsLog.Info($"=====> PROMETHEUS METRICS SCRAPE REQUEST");
            var _registry = Prometheus.Metrics.DefaultRegistry;
            using (var stream = new MemoryStream())
            {
                await _registry.CollectAndExportAsTextAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    var metricsText = await reader.ReadToEndAsync();
                    _metricsLog.Info($"<==== PROMETHEUS METRICS SCRAPE COMPLETE");
                    return Content(metricsText, "text/plain; version=0.0.4");
                }
            }
        }
    }
}
