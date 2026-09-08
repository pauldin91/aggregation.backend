using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Statistics;
using Aggregation.Backend.Infrastructure.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Aggregation.Backend.WebApi.Controllers
{
    [ApiController]
    public class StatisticsController(PerformanceStatisticsCache performanceStatisticsCache) : ControllerBase
    {
        /// <summary>
        /// Provides performance statistics for incoming API requests.
        /// </summary>
        /// <remarks>
        /// This endpoint returns metrics such as total request count, average response time, and a breakdown of request durations categorized in Fast,Average and Slow based on their average response time.
        /// Useful for monitoring and diagnostics.
        /// </remarks>
        /// <returns>
        /// A <see cref="RequestStatisticsDto"/> object containing performance metrics.
        /// </returns>
        /// <response code="200">Returns the current performance statistics.</response>
        [HttpGet(ApiEndpoints.GetStatisticsRoute)]
        [ProducesResponseType(typeof(RequestStatisticsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestStatistics()
        {
            return Ok(performanceStatisticsCache.GetStats());
        }
    }
}