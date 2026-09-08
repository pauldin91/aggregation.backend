using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Infrastructure.Cache;
using System.Diagnostics;

namespace Aggregation.Backend.WebApi.Middlewares
{
    public class RequestAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PerformanceStatisticsCache _performanceStatisticsCache;

        public RequestAnalyticsMiddleware(RequestDelegate next, PerformanceStatisticsCache performanceStatisticsCache)
        {
            _next = next;
            _performanceStatisticsCache = performanceStatisticsCache;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();
            if (context.Request.Path.ToString().Contains(ApiEndpoints.Aggregates))
                _performanceStatisticsCache.Record(stopwatch.ElapsedMilliseconds);
        }
    }
}