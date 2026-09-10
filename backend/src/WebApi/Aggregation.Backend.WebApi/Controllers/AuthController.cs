using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using Aggregation.Backend.Application.Features.Aggregates;
using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Aggregates;
using Aggregation.Backend.Infrastructure.Options;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using NuGet.Packaging.Signing;

namespace Aggregation.Backend.WebApi.Controllers
{
    public class CallbackBody
    {
        public string Code { get; set; }
        public string Iss { get; set; }
    }
    [AllowAnonymous]
    [ApiController]
    public class AuthController(IOptions<JwtOptions> tokenOptions) : ControllerBase
    {
        [HttpPost(ApiEndpoints.Callback)]
        [ProducesResponseType(typeof(List<AggregatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExchangeCode([FromBody] CallbackBody? body, CancellationToken cancellationToken)
        {

            Console.WriteLine("got code: ", body.Code);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.Value.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
{
                new Claim(JwtRegisteredClaimNames.Sub, "user123"),      // Subject (User ID)
                new Claim(JwtRegisteredClaimNames.Email, "user@example.com"),
                new Claim(JwtRegisteredClaimNames.Name, "John Doe"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
                new Claim("role", "admin"),                              // Custom claims
                new Claim("permission", "read"),
                new Claim("permission", "write")
            };
            var token = new JwtSecurityToken(
                issuer: "https://myapp.com",              // Where it came from
                audience: "https://myapi.com",            // Who can use it
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),    // 1 hour expiration
                signingCredentials: credentials
            );
            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);

            return Ok(new { AccessToken = tokenString });
        }
    }
}