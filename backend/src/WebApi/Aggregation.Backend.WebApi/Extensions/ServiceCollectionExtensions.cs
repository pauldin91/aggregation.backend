using Aggregation.Backend.Domain.Constants;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.WebApi.Policies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Reflection;

namespace Aggregation.Backend.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApiExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerDefinitions(configuration);

            services.AddOutputCache(s =>
            {
                s.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(10);
                s.AddPolicy(Domain.Constants.Policies.AggregatesCachePolicy, builder =>

                     builder.AddPolicy<AuthenticatedCachePolicy>()
                     .SetVaryByQuery(Domain.Constants.Policies.KeywordQueryParam, Domain.Constants.Policies.FilterQueryParam, Domain.Constants.Policies.SortQueryParam, Domain.Constants.Policies.SortTypeQueryParam)
                 , true);
            });

            return services;
        }

        private static IServiceCollection AddSwaggerDefinitions(this IServiceCollection services, IConfiguration configuration)
        {
            var extIdOptions = new ExternalIdProviderOptions();
            configuration.Bind(nameof(ExternalIdProviderOptions), extIdOptions);


            services.AddSwaggerGen(options =>
            {
                var version = $"v{ApiEndpoints.Version}";

                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = "Aggregation API",
                    Version = version,
                    Description = "Secure API with JWT Authentication"
                });

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                // options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                // {
                //     Type = SecuritySchemeType.OAuth2,
                //     Flows = new OpenApiOAuthFlows
                //     {
                //         AuthorizationCode = new OpenApiOAuthFlow
                //         {
                //             AuthorizationUrl = new Uri(extIdOptions.AuthorizationEndpoint),
                //             TokenUrl = new Uri(extIdOptions.TokenEndpoint),
                //             Scopes = new Dictionary<string, string>
                //              {
                //                  { "read:user", "Read user profile" },
                //                  {"user:email","Read user email" }
                //              }
                //         }
                //     }
                // });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme,
                            }
                        },
                        Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "OAuth2"
                            }
                        },
                        new[] { "openid", "profile" }
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}