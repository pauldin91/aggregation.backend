using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Statistics;
using Aggregation.Backend.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Aggregation.Backend.Infrastructure.Cache
{
    public class PerformanceStatisticsCache(IOptions<BucketOptions> bucketOptions)
    {
        private static int _requestCount = 0;
        private static long _accumulativeReponseTime = 0;
        private static readonly ConcurrentDictionary<string, Tuple<long, int>> _responseTimes = new();
        public static IReadOnlyDictionary<string, Tuple<long, int>> ResponseTimes => _responseTimes.ToDictionary(s => s.Key, v => v.Value);

        public static int TotalRequestCount => _requestCount;

        public static long AverageResponseTime
        {
            get
            {
                if (_requestCount == 0)
                {
                    return 0;
                }
                else
                {
                    return _accumulativeReponseTime / _requestCount;
                }
            }
        }

        public void Record(long durationMs)
        {
            Interlocked.Increment(ref _requestCount);
            Interlocked.Add(ref _accumulativeReponseTime, durationMs);

            string key = durationMs switch
            {
                var fast when fast < bucketOptions.Value.FastUpperLimit => Buckets.Fast,
                var avg when avg <= bucketOptions.Value.AverageUpperLimit => Buckets.Average,
                _ => Buckets.Slow
            };

            _responseTimes.AddOrUpdate(key,
                Tuple.Create(durationMs, 1),
                (id, existing) => Tuple.Create(existing.Item1 + durationMs, existing.Item2 + 1)
            );
        }

        public RequestStatisticsDto GetStats()
        {
            var result = new RequestStatisticsDto
            {
                AverageResponseTimeMs = AverageResponseTime,
                TotalRequests = TotalRequestCount,
                Buckets = _responseTimes.ToDictionary(s => s.Key, s => s.Value.Item2)
            };

            return result;
        }
    }
}