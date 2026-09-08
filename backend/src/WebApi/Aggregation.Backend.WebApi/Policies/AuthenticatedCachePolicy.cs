using Aggregation.Backend.Infrastructure.Options;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Aggregation.Backend.WebApi.Policies
{
    public class AuthenticatedCachePolicy(IOptions<JwtOptions> jwtOptions) : IOutputCachePolicy
    {
        public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation) { 
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Split(' ');
            if (token.Length == 2 ) 
            {
                context.EnableOutputCaching = true;
                context.AllowCacheLookup = true;
                context.AllowCacheStorage = true;
            }
            return ValueTask.CompletedTask;
        }

        public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation) => ValueTask.CompletedTask;

        public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation) => ValueTask.CompletedTask;

        public async Task<bool> ValidateToken(string token) {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParams = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Value.Issuer,
                ValidAudience = jwtOptions.Value.Audience,
                ValidAudiences = [ jwtOptions.Value.Audience ],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)),
                ClockSkew = TimeSpan.FromSeconds(5),
            };
            var result = await handler.ValidateTokenAsync(token, tokenValidationParams);
            return result.IsValid;
        }
    }
}
