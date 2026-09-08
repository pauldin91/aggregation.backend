using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Dtos.External.Air;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;

namespace Agregation.Backend.Infrastructure.Tests.Services
{
    public class AirPollutionServiceTests
    {
        private Mock<IHttpClientWrapper<AirPollutionOptions>> _httpClientMock = null!;
        private AirPollutionService _service = null!;
        private AirPollutionOptions _options = null!;

        [SetUp]
        public void Setup()
        {
            _httpClientMock = new Mock<IHttpClientWrapper<AirPollutionOptions>>();

            _options = new AirPollutionOptions
            {
                ApiKey = "test-key",
                ListUri = "list",
                BaseUrl = "air",
                GetUri = "get",
            };

            var optionsMock = new Mock<IOptions<AirPollutionOptions>>();
            optionsMock.Setup(o => o.Value).Returns(_options);

            _service = new AirPollutionService(_httpClientMock.Object, optionsMock.Object);
        }

        [Test]
        public async Task ListAsync_ReturnsMappedCityData_WhenResponsesAreValid()
        {
            // Arrange
            var category = "Attica";
            var cancellationToken = CancellationToken.None;

            var citiesDto = new CitiesDto
            {
                Cities = new List<NameOnlyDto>
            {
                new NameOnlyDto { City = "Athens" },
                new NameOnlyDto { City = "Piraeus" }
            }
            };

            var cityDtoAthens = new CityDto
            {
                Data = new() { City = "Athens", Current = new() { Pollution = new() { Aqius = 42 } } }
            };

            var cityDtoPiraeus = new CityDto
            {
                Data = new() { City = "Piraeus", Current = new() { Pollution = new() { Aqius = 55 } } }
            };

            var citiesJson = JsonConvert.SerializeObject(citiesDto);
            var athensJson = JsonConvert.SerializeObject(cityDtoAthens);
            var piraeusJson = JsonConvert.SerializeObject(cityDtoPiraeus);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.StartsWith(_options.ListUri)), cancellationToken))
                .ReturnsAsync(citiesJson);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.Contains("Athens")), cancellationToken))
                .ReturnsAsync(athensJson);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.Contains("Piraeus")), cancellationToken))
                .ReturnsAsync(piraeusJson);

            // Act
            var result = await _service.ListAsync(category, cancellationToken);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(r => r["City"].ToString() == "Athens"));
            Assert.That(result.Any(r => r["City"].ToString() == "Piraeus"));
        }

        [Test]
        public async Task ListAsync_WhenCityResponseIsEmpty_ReturnsOnlyValidCities()
        {
            // Arrange
            var category = "Attica";
            var cancellationToken = CancellationToken.None;

            var citiesDto = new CitiesDto
            {
                Cities = new List<NameOnlyDto>
            {
                new NameOnlyDto { City = "Athens" },
                new NameOnlyDto { City = "GhostTown" }
            }
            };

            var cityDtoAthens = new CityDto
            {
                Data = new() { City = "Athens", Current = new() { Pollution = new() { Aqius = 42 } } },
            };

            var citiesJson = JsonConvert.SerializeObject(citiesDto);
            var athensJson = JsonConvert.SerializeObject(cityDtoAthens);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.StartsWith(_options.ListUri)), cancellationToken))
                .ReturnsAsync(citiesJson);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.Contains("Athens")), cancellationToken))
                .ReturnsAsync(athensJson);

            _httpClientMock
                .Setup(h => h.GetAsync(It.Is<string>(s => s.Contains("GhostTown")), cancellationToken))
                .ReturnsAsync(string.Empty); // Simulate empty response

            // Act
            var result = await _service.ListAsync(category, cancellationToken);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0]["City"].ToString(), Is.EqualTo("Athens"));
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