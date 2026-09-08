using System.Collections.Concurrent;

namespace Aggregation.Backend.Infrastructure.Cache
{
    public class ExternalApiRequestTimingCache
    {
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<Tuple<DateTime, long>>> _responseTimes = new();

        public void Record(string endpoint, long durationMs)
        {
            var queue = _responseTimes.GetOrAdd(endpoint, _ => new ConcurrentQueue<Tuple<DateTime, long>>());
            queue.Enqueue(Tuple.Create(DateTime.UtcNow, durationMs));

            while (queue.TryPeek(out var entry) && entry.Item1 < DateTime.UtcNow.AddMinutes(-5))
            {
                queue.TryDequeue(out _);
            }
        }

        public IReadOnlyDictionary<string, long> GetResponseTimes()
        {
            return _responseTimes.ToDictionary(s => s.Key, l =>
            {
                if (l.Value.Count == 0)
                {
                    return 0;
                }
                return l.Value.Sum(p => p.Item2) / l.Value.Count;
            });
        }
    }
}