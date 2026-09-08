using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Infrastructure.Cache;

namespace Agregation.Backend.Infrastructure.Tests.Cache
{
    public class ExternalApiRequestTimingCacheTests
    {
        private ExternalApiRequestTimingCache _cache = null!;

        public ExternalApiRequestTimingCacheTests()
        {
            _cache = new ExternalApiRequestTimingCache();
            _cache.Record("external_api_1", 50);
            _cache.Record("external_api_1", 90);
            _cache.Record("external_api_1", 130);
            _cache.Record("external_api_2", 150);
            _cache.Record("external_api_2", 160);
            _cache.Record("external_api_2", 180);
        }

        [Test]
        public void Record_AddsToCacheBasedOnEndpoint()
        {
            var cache = _cache.GetResponseTimes();
            Assert.That(cache.Count, Is.EqualTo(2));
            _cache.Record("external_api_3", 250);
            cache = _cache.GetResponseTimes();
            Assert.That(cache.Count, Is.EqualTo(3));

        }

        [Test]
        public void GetResponseTimes_ReturnsAverageTimesPerEndpointCorrectly()
        {
            var cache = _cache.GetResponseTimes();

            Assert.That(cache["external_api_1"], Is.EqualTo((50+90+130)/3));
            Assert.That(cache["external_api_2"], Is.EqualTo((150+160+180)/3));
        }
    }
}