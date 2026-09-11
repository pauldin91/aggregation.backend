using Aggregation.Backend.Domain.Dtos.Auth;
using Aggregation.Backend.Domain.Interfaces;
using Aggregation.Backend.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aggregation.Backend.Infrastructure.Helpers
{
    public class TokenGenerator(IOptions<JwtOptions> options) : ITokenGenerator
    {
        public TokenResponse GenerateToken(UserInfoResponse userInfo)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Name, userInfo.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new TokenResponse
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Exp = token.IssuedAt.AddHours(1)
            };
        }
    }
}