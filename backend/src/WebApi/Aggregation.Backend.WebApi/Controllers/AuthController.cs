using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using Aggregation.Backend.Application.Features.Aggregates;
using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Domain.Dtos.Aggregates;
using Aggregation.Backend.Domain.Dtos.Auth;
using Aggregation.Backend.Infrastructure.Options;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using NuGet.Common;
using NuGet.Packaging.Signing;
using NuGet.Protocol;

namespace Aggregation.Backend.WebApi.Controllers
{
    public class CallbackBody
    {
        public string Code { get; set; }
        public string Iss { get; set; }
    }

    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly HttpClient _client;
        private readonly IHttpOAuth2ClientOptions _extIdProvider;
        private readonly JwtOptions _tokenOptions;
        public AuthController(IHttpClientFactory factory, IConfiguration configuration)
        {
            _client = factory.CreateClient(typeof(ExternalIdProviderOptions).Name);
            _extIdProvider = ConfigurationBinder.Get<ExternalIdProviderOptions>(configuration.GetSection(typeof(ExternalIdProviderOptions).Name));
            _tokenOptions = ConfigurationBinder.Get<JwtOptions>(configuration.GetSection(typeof(JwtOptions).Name));
        }
        [HttpPost(ApiEndpoints.Callback)]
        [ProducesResponseType(typeof(List<AggregatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExchangeCode([FromBody] CallbackBody? body, CancellationToken cancellationToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenResponse = await _client.PostAsync(string.Format(_extIdProvider.TokenUrl, _extIdProvider.ClientId), new FormUrlEncodedContent(new Dictionary<string, string>
            {
               { "client_id", _extIdProvider.ClientId},
                {"client_secret", _extIdProvider.ClientSecret},
                {"code", body.Code},
            }), cancellationToken);
            var tokenResult = await tokenResponse.Content.ReadAsAsync<TokenResponse>();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "user");
            requestMessage.Headers.Add(HeaderNames.Authorization, string.Format("{0} {1}", "Bearer", tokenResult.AccessToken));
            var userInfoResponse = await _client.SendAsync(requestMessage,cancellationToken);
            var userResult = await userInfoResponse.Content.ReadAsAsync<UserInfoResponse>();


            var claims = new List<Claim>
{
                new Claim(JwtRegisteredClaimNames.Sub, userResult.Id.ToString()),      // Subject (User ID)
                new Claim(JwtRegisteredClaimNames.Email, userResult.Email),
                new Claim(JwtRegisteredClaimNames.Name, userResult.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique token ID
            };
            var token = new JwtSecurityToken(
                issuer: Request.GetDisplayUrl(),              // Where it came from
                audience: Request.GetDisplayUrl(),            // Who can use it
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),    // 1 hour expiration
                signingCredentials: credentials
            );
            var handler = new JwtSecurityTokenHandler();
            string tokenString = handler.WriteToken(token);

            return Ok(new { AccessToken = tokenString, GithubToken = tokenResponse });
        }
    }
}