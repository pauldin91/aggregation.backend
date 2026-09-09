using System.Security.Permissions;
using Aggregation.Backend.Application.Features.Aggregates;
using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Aggregates;
using Aggregation.Backend.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;

namespace Aggregation.Backend.WebApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class AuthController(IOptions<ExternalIdProviderOptions> extIdOptions) : ControllerBase
    {
        [HttpGet(ApiEndpoints.Callback)]
        [ProducesResponseType(typeof(List<AggregatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExchangeCode([FromQuery] string? code, [FromQuery] string? iss, CancellationToken cancellationToken)
        {
            Console.WriteLine("Received code {0} and issuer {1}", code, iss);

            return Ok(Response.Body);
        }
    }
}