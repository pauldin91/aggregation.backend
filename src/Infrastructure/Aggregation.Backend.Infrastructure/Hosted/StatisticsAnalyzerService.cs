﻿using Aggregation.Backend.Infrastructure.Cache;
using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aggregation.Backend.Infrastructure.Hosted
{
    public class StatisticsAnalyzerService(ILogger<StatisticsAnalyzerService> logger) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            RecurringJob.AddOrUpdate(nameof(StatisticsAnalyzerService), () => CollectPerformanceData(), "* * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
            return Task.CompletedTask;
        }

        public void CollectPerformanceData()
        {
            try
            {
                var responseTimes = ExternalApiRequestTimingCache.GetResponseTimes();
                foreach (var responseTime in responseTimes)
                {
                    var avgPerformance = PerformanceStatisticsCache.AverageResponseTime;
                    if (responseTime.Value > 1.5 * avgPerformance)
                    {
                        logger.LogWarning("Detected degraded performance of endpoint {Endpoint} with average reponse time  of {EndpointResponseTime} over the last 5 minutes, while average performance is {AvgReponseTime}",
                            responseTime.Key, responseTime.Value, avgPerformance);
                    }
                }
            }
            catch
            {
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}