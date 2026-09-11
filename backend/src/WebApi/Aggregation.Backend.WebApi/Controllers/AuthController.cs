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
using Aggregation.Backend.Domain.Interfaces;
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

        private readonly ITokenGenerator _tokenGenerator;
        public AuthController(IHttpClientFactory factory, IConfiguration configuration, ITokenGenerator tokenGenerator)
        {
            _client = factory.CreateClient(typeof(ExternalIdProviderOptions).Name);
            _extIdProvider = ConfigurationBinder.Get<ExternalIdProviderOptions>(configuration.GetSection(typeof(ExternalIdProviderOptions).Name))!;
            _tokenOptions = ConfigurationBinder.Get<JwtOptions>(configuration.GetSection(typeof(JwtOptions).Name))!;
            _tokenGenerator = tokenGenerator;
        }
        [HttpPost(ApiEndpoints.Callback)]
        [ProducesResponseType(typeof(List<AggregatedResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExchangeCode([FromBody] CallbackBody? body, CancellationToken cancellationToken)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _extIdProvider.TokenUrl)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _extIdProvider.ClientId },
                    { "client_secret", _extIdProvider.ClientSecret },
                    { "code", body.Code },
                })
            };
            tokenRequest.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var tokenResponse = await _client.SendAsync(tokenRequest, cancellationToken);
            tokenResponse.EnsureSuccessStatusCode();

            var tokenBody = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
            var tokenResult = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(tokenBody)!;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "user");
            requestMessage.Headers.Add(HeaderNames.Authorization, string.Format("{0} {1}", "Bearer", tokenResult.AccessToken));

            var userInfoResponse = await _client.SendAsync(requestMessage, cancellationToken);
            userInfoResponse.EnsureSuccessStatusCode();

            var userResult = await userInfoResponse.Content.ReadAsAsync<UserInfoResponse>();

            var token = _tokenGenerator.GenerateToken(userResult);

            return Ok(new { AccessToken = token });




        }
    }
}