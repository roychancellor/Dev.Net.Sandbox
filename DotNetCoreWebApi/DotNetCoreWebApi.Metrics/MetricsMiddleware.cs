using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Diagnostics.Metrics;
using Prometheus;

namespace DotNetCoreWebApi.Metrics
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private static long _totalRequests;
        private static Stopwatch _stopwatch = Stopwatch.StartNew();
        private static ConcurrentDictionary<string, long> _requestsByIp = new ConcurrentDictionary<string, long>();
        private static ConcurrentDictionary<string, long> _requestsByXForwardedFor = new ConcurrentDictionary<string, long>();
        private readonly Counter _totalRequestsCounter = Prometheus.Metrics.CreateCounter("myapp_total_requests", "Total number of requests received.");
        private readonly Gauge _currentRequestsGauge = Prometheus.Metrics.CreateGauge("myapp_current_requests", "Current number of active requests.");

        public MetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Prometheus metrics
            _totalRequestsCounter.Inc();
            _currentRequestsGauge.Inc();

            Interlocked.Increment(ref _totalRequests);

            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            _requestsByIp.AddOrUpdate(ipAddress, 1, (key, value) => value + 1);

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xForwardedForHeader))
            {
                var xForwardedFor = xForwardedForHeader.ToString();
                _requestsByXForwardedFor.AddOrUpdate(xForwardedFor, 1, (key, value) => value + 1);
            }

            try
            {
                await _next(context);
            }
            finally
            {
                _currentRequestsGauge.Dec();
            }
        }

        public static double AverageRequestsPerSecond => _totalRequests / _stopwatch.Elapsed.TotalSeconds;

        public static Metrics GetMetrics()
        {
            return new Metrics
            {
                TotalRequests = _totalRequests,
                AverageRequestsPerSecond = AverageRequestsPerSecond,
                RequestsByIp = _requestsByIp.ToDictionary(kv => kv.Key, kv => kv.Value),
                RequestsByXForwardedFor = _requestsByXForwardedFor.ToDictionary(kv => kv.Key, kv => kv.Value)
            };
        }
    }

    public class Metrics
    {
        public long TotalRequests { get; set; }
        public double AverageRequestsPerSecond { get; set; }
        public Dictionary<string, long> RequestsByIp { get; set; }
        public Dictionary<string, long> RequestsByXForwardedFor { get; set; }

        public Metrics()
        {
            RequestsByIp = new Dictionary<string, long>();
            RequestsByXForwardedFor = new Dictionary<string, long>();
        }
    }
}
