using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Infrastructure.Cache;

namespace Aggregation.Backend.Infrastructure.Tests.Cache
{
    public class ExternalApiRequestTimingCacheTests
    {

        public ExternalApiRequestTimingCacheTests()
        {
            ExternalApiRequestTimingCache.Record("external_api_1", 50);
            ExternalApiRequestTimingCache.Record("external_api_1", 90);
            ExternalApiRequestTimingCache.Record("external_api_1", 130);
            ExternalApiRequestTimingCache.Record("external_api_2", 150);
            ExternalApiRequestTimingCache.Record("external_api_2", 160);
            ExternalApiRequestTimingCache.Record("external_api_2", 180);
        }

        [Test]
        public void Record_AddsToCacheBasedOnEndpoint()
        {
            var cache = ExternalApiRequestTimingCache.GetResponseTimes();
            Assert.That(cache.Count, Is.EqualTo(2));
            ExternalApiRequestTimingCache.Record("external_api_3", 250);
            cache = ExternalApiRequestTimingCache.GetResponseTimes();
            Assert.That(cache.Count, Is.EqualTo(3));

        }

        [Test]
        public void GetResponseTimes_ReturnsAverageTimesPerEndpointCorrectly()
        {
            var cache = ExternalApiRequestTimingCache.GetResponseTimes();

            Assert.That(cache["external_api_1"], Is.EqualTo((50+90+130)/3));
            Assert.That(cache["external_api_2"], Is.EqualTo((150+160+180)/3));
        }
    }
}