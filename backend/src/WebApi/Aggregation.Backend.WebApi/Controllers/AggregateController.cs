using Aggregation.Backend.Application.Features.Aggregates;
using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Aggregates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Aggregation.Backend.WebApi.Controllers
{
    [ApiController]
    public class AggregateController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Retrieves aggregated news items based on a keyword and optional filters.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a list of aggregated results, which may include news from https://newsapi.org/v2/,
        /// cities pollution based on their belonging state from http://api.airvisual.com/v2/ or
        /// stock market feeds based on the Stock symbol from https://www.alphavantage.co/.
        /// Use query parameters to refine the search and control sorting behavior.
        /// </remarks>
        /// <param name="keyword">
        /// Required keyword used to search across sources. This is the main term used to retrieve relevant content.
        /// Specifically for Pollution External Api search the keyword reflects to the states of Greece and must one of the following:
        /// <c> Attica, Central Greece, Central Macedonia, Crete, East Macedonia and Thrace, Epirus, Ionian Islands, North Aegean, Peloponnese, South Aegean, Thessaly, West Greece, West Macedonia </c>
        /// </param>
        /// <param name="filterBy">
        /// Optional filter to narrow down results. Use the format <c>FieldName=Value</c> to specify filtering criteria.
        /// For example: <c>Author=AuthorNameHere</c> will return resources authored by "AuthorNameHere".
        /// Supported fields may include nested properties like <c>Source.Name=NameOfSource</c> or <c>Category.Name=NameOfCategory</c> or <c>Current.Pollution.Ts=2025-08-10T14:00:00Z</c>.
        /// Multiple filters are not supported in a single query string.
        /// </param>
        /// <param name="orderBy">
        /// Field name to sort results by. Common values include <c>date</c>, <c>title</c>, or <c>relevance</c>.
        /// Sorting is applied after filtering.
        /// </param>
        /// <param name="asc">
        /// Determines sort direction. Set to <c>true</c> for ascending order or <c>false</c> for descending.
        /// Default is <c>true</c>. Applies to the field specified in <paramref name="orderBy"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// Token to cancel the request if needed. Useful for long-running operations or client-side timeouts.
        /// </param>
        /// <returns>
        /// A list of aggregated responses. Each response may contain an article, a city, or a feed item.
        /// </returns>
        /// <response code="200">Returns the aggregated results successfully.</response>
        /// <response code="400">Invalid query parameters or missing keyword.</response>
        /// <response code="500">Unexpected server error occurred.</response>
        [HttpGet(ApiEndpoints.GetAggregatesRoute)]
        [ProducesResponseType(typeof(List<AggregatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OutputCache(PolicyName = Domain.Constants.Policies.AggregatesCachePolicy)]
        public async Task<IActionResult> GetNewsAsync([FromQuery] string keyword, [FromQuery] string? filterBy, [FromQuery] string? orderBy, CancellationToken cancellationToken, [FromQuery] bool asc = true)
        {
            var result = await mediator.Send(new AggregatesQuery(keyword, filterBy, orderBy, asc), cancellationToken);

            return Ok(result);
        }
    }
}