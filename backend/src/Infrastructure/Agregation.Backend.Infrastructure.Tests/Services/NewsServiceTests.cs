using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Dtos.External.Air;
using Aggregation.Backend.Domain.Dtos.External.News;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace Agregation.Backend.Infrastructure.Tests.Services
{
    public class NewsServiceTests
    {
        private Mock<IHttpClientWrapper<NewsOptions>> _httpClientMock = null!;
        private NewsService _service = null!;
        private NewsOptions _options = null!;

        public NewsServiceTests()
        {
            _httpClientMock = new Mock<IHttpClientWrapper<NewsOptions>>();

            _options = new NewsOptions
            {
                ApiKey = "test-key",
                BaseUrl = "news",
                ListUri = "list",
            };

            var optionsMock = new Mock<IOptions<NewsOptions>>();
            optionsMock.Setup(o => o.Value).Returns(_options);

            _service = new NewsService(_httpClientMock.Object, optionsMock.Object);
        }

        [Test]
        public async Task ListAsync_ReturnsMappedArticles_WhenResponsesAreValid()
        {
            var category = "api";
            var cancellationToken = CancellationToken.None;

            var news = new ArticlesDto
            {
                Articles = new() {
                    new(){Author="BBC Author" ,Title="Software Development",Source=new(){ Name="BBC"} },
                    new(){Author="e-learning Author" ,Title="Advance in tech",Source=new(){ Name="e-learning platform"} },
                }
            };

            var newsJson = JsonConvert.SerializeObject(news);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.StartsWith(_options.ListUri)), cancellationToken))
                .ReturnsAsync(newsJson);

            var result = await _service.ListAsync(category, cancellationToken);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(r => r["Author"].ToString() == "BBC Author"));
            Assert.That(result.Any(r => r["Author"].ToString() == "e-learning Author"));
            Assert.That(result.Any(r => r["Title"].ToString() == "Software Development"));
            Assert.That(result.Any(r => r["Title"].ToString() == "Advance in tech"));
            Assert.That(result.Any(r => r["Source.Name"].ToString() == "BBC"));
            Assert.That(result.Any(r => r["Source.Name"].ToString() == "e-learning platform"));
        }


        [Test]
        public async Task ListAsync_WhenInitialResponseIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var category = "invalid";
            var cancellationToken = CancellationToken.None;

            _httpClientMock
                .Setup(h => h.GetAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(string.Empty);

            // Act
            var result = await _service.ListAsync(category, cancellationToken);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}