using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Dtos.External.StockMarket;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace Agregation.Backend.Infrastructure.Tests.Services
{
    public class StockMarketFeedServiceTests
    {
        private Mock<IHttpClientWrapper<StockMarketFeedOptions>> _httpClientMock = null!;
        private StockMarketFeedService _service = null!;
        private StockMarketFeedOptions _options = null!;

        public StockMarketFeedServiceTests()
        {
            _httpClientMock = new Mock<IHttpClientWrapper<StockMarketFeedOptions>>();

            _options = new StockMarketFeedOptions
            {
                ApiKey = "test-key",
                BaseUrl = "feed",
                ListUri = "list",
            };

            var optionsMock = new Mock<IOptions<StockMarketFeedOptions>>();
            optionsMock.Setup(o => o.Value).Returns(_options);

            _service = new StockMarketFeedService(_httpClientMock.Object, optionsMock.Object);
        }

        [Test]
        public async Task ListAsync_ReturnsMappedFeed_WhenResponsesAreValid()
        {
            var category = "api";
            var cancellationToken = CancellationToken.None;

            var feedDto = new StockMarketFeedDto
            {
                Feed = new() {
                new(){
                    Authors=["Finace","Stoks"],
                    Source="investing-platform",
                    TickerSentiment=  new() {
                        new(){Ticker = "STOCK_1_SYMBOL"},
                        new(){Ticker = "STOCK_2_SYMBOL"}
                    }
                 },
                new(){
                    Authors=["bbc","nfc"],
                    Source="beting-platform",
                    TickerSentiment=  new() {
                        new(){Ticker = "STOCK_3_SYMBOL"},
                        new(){Ticker = "STOCK_4_SYMBOL"}
                    }
                 }
               }
            };

            var newsJson = JsonConvert.SerializeObject(feedDto);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.StartsWith(_options.ListUri)), cancellationToken))
                .ReturnsAsync(newsJson);

            var result = await _service.ListAsync(category, cancellationToken);

            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(result.Any(r => r["Source"].ToString() == "investing-platform"));
            Assert.That(result.Any(r => r["Source"].ToString() == "beting-platform"));

            var authors = result.SelectMany(s=>s["Authors"] as List<string>);
            Assert.That(authors.Any(r => r.Equals("Stoks")));
            Assert.That(authors.Any(r => r.Equals("Finace")));
            Assert.That(authors.Any(r => r.Equals("bbc")));
            Assert.That(authors.Any(r => r.Equals("nfc")));

            var tickers = result.SelectMany(s => s["TickerSentiment"] as List<Dictionary<string, object>>).Select(s => s["Ticker"].ToString()).ToList();
            Assert.That(tickers.Any(s => s.Equals("STOCK_4_SYMBOL")));
            Assert.That(tickers.Any(s => s.Equals("STOCK_3_SYMBOL")));
            Assert.That(tickers.Any(s => s.Equals("STOCK_2_SYMBOL")));
            Assert.That(tickers.Any(s => s.Equals("STOCK_1_SYMBOL")));

        }

        [Test]
        public async Task ListAsync_WhenInitialResponseIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var category = "Attica";
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