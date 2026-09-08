using Aggregation.Backend.Infrastructure.Cache;
using Aggregation.Backend.Infrastructure.Services;
using Agregation.Backend.Infrastructure.Tests.Helpers;
using Moq;
using Moq.Protected;
using System.Net;

namespace Agregation.Backend.Infrastructure.Tests.Services
{
    public class HttpClientWrapperTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock = null!;
        private Mock<ExternalApiRequestTimingCache> _timingCacheMock = null!;
        private HttpClientWrapper<MockOptions> _wrapper = null!;
        private Mock<HttpMessageHandler> _handlerMock = null!;
        private MockOptions _options = new() { BaseUrl = "https://api.example.com" };

        public HttpClientWrapperTests()
        {
            _handlerMock = new Mock<HttpMessageHandler>();

            var httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new System.Uri(_options.BaseUrl)
            };

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock
                .Setup(f => f.CreateClient(typeof(MockOptions).Name))
                .Returns(httpClient);

            _timingCacheMock = new Mock<ExternalApiRequestTimingCache>();

            _wrapper = new HttpClientWrapper<MockOptions>(_httpClientFactoryMock.Object, _timingCacheMock.Object);
        }

        [Test]
        public async Task GetAsync_WhenResponseIsSuccessful_ReturnsContentAndRecordsTiming()
        {
            var expectedContent = "{\"status\":\"ok\"}";
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedContent)
            };

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);


            var result = await _wrapper.GetAsync("/data", CancellationToken.None);

            Assert.That(result, Is.EqualTo(expectedContent));
        }

        [Test]
        public async Task GetAsync_WhenResponseFails_ReturnsEmptyStringAndRecordsTiming()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Something went wrong")
            };

            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);


            var result = await _wrapper.GetAsync("/data", CancellationToken.None);

            Assert.That(result, Is.Empty);
            
        }
    }
}