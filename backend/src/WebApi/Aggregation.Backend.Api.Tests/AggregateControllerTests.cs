using Aggregation.Backend.Application.Features.Aggregates;
using Aggregation.Backend.Domain.Dtos.Aggregates;
using Aggregation.Backend.WebApi.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Aggregation.Backend.Api.Tests
{
    public class AggregateControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AggregateController _controller;

        public AggregateControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AggregateController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetNewsAsync_ReturnsOkResult_WithAggregatedResponses()
        {
            var keyword = "Greece";
            var filterBy = "Source.Name=BBC";
            var orderBy = "Author";
            var asc = true;
            var cancellationToken = CancellationToken.None;

            var response = new List<AggregatedResponse>
            {
                new AggregatedResponse { Article = new(){Title = "Sample News", Source = new (){ Name = "BBC" } } }
            };
            var expectedResponse = response.Select(s => s.Article?.ToMap()).ToList();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AggregatesQuery>(), cancellationToken))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.GetNewsAsync(keyword, filterBy, orderBy, cancellationToken, asc);

            var okResult = result as OkObjectResult;
            Assert.IsAssignableFrom<List<Dictionary<string, object>>>(okResult.Value);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(((List<Dictionary<string, object>>)okResult.Value)[0]["Source.Name"], Is.EqualTo("BBC"));
        }

        [Test]
        public async Task GetNewsAsync_MediatorThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AggregatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await _controller.GetNewsAsync("Greece", null, null, CancellationToken.None, true);
            });

        }
    }
}