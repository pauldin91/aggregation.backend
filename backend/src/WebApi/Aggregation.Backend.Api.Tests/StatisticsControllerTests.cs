using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Statistics;
using Aggregation.Backend.Infrastructure.Cache;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

public class StatisticsControllerTests
{
    private StatisticsController _controller = null!;
    private PerformanceStatisticsCache _cache = null!;
    private Mock<IOptions<BucketOptions>> _optionsMock = null;


    public StatisticsControllerTests()
    {
        var bucketOptions = new BucketOptions
        {
            FastUpperLimit = 100,
            AverageUpperLimit = 200
        };

        _optionsMock = new Mock<IOptions<BucketOptions>>();
        _optionsMock.Setup(o => o.Value).Returns(bucketOptions);
        _cache = new PerformanceStatisticsCache(_optionsMock.Object);

        _controller = new StatisticsController(_cache);
        _cache.Record(50);
        _cache.Record(150);
        _cache.Record(300);
    }

    [Test]
    public async Task GetRequestStatistics_ReturnsOkWithCorrectStats()
    {
        var result = await _controller.GetRequestStatistics();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var stats = okResult!.Value as RequestStatisticsDto;
        Assert.That(stats, Is.Not.Null);
        Assert.That(stats!.TotalRequests, Is.EqualTo(3));
        Assert.That(stats.Buckets[Buckets.Fast], Is.EqualTo(1));
        Assert.That(stats.Buckets[Buckets.Average], Is.EqualTo(1));
        Assert.That(stats.Buckets[Buckets.Slow], Is.EqualTo(1));
        Assert.That(stats.AverageResponseTimeMs, Is.EqualTo((50 + 150 + 300) / 3));
    }

    
}