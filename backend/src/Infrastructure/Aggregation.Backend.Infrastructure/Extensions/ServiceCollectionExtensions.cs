using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Entities;
using Aggregation.Backend.Infrastructure.Cache;
using Aggregation.Backend.Infrastructure.Data.Contexts;
using Aggregation.Backend.Infrastructure.Helpers;
using Aggregation.Backend.Infrastructure.Hosted;
using Aggregation.Backend.Infrastructure.Options;
using Aggregation.Backend.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Aggregation.Backend.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(cfg => { cfg.UseInMemoryStorage(); });
            services.AddHangfireServer();


            var jwtOptions = new JwtOptions();
            configuration.Bind(nameof(JwtOptions), jwtOptions);

            var extIdOptions = new ExternalIdProviderOptions();
            configuration.Bind(nameof(ExternalIdProviderOptions), extIdOptions);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddGitHub(options =>
              {
                  options.ClientId = extIdOptions.ClientId;
                  options.ClientSecret = extIdOptions.ClientSecret;
                  options.Scope.Add("user:email");
              });

            services.AddHttpClientFromOptions(configuration);

            services.Configure<StatisticsAnalyzerServiceOptions>(configuration.GetSection(nameof(StatisticsAnalyzerServiceOptions)));
            services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            services.Configure<BucketOptions>(configuration.GetSection(nameof(BucketOptions)));
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<IAirPollutionService, AirPollutionService>();
            services.AddTransient<IStockMarketFeedService, StockMarketFeedService>();
            services.AddHostedService<StatisticsAnalyzerService>();
            services.AddScoped<TokenGenerator>();
            services.AddSingleton<ExternalApiRequestTimingCache>();
            services.AddSingleton<PerformanceStatisticsCache>();

            return services;
        }
        
        private static IServiceCollection AddHttpClientFromOptions(this IServiceCollection services,IConfiguration configuration)
        {
            var allOptions = typeof(NewsOptions).Assembly.GetTypes()
                .Where(s => s.IsAssignableTo(typeof(IHttpClientOptions)))
                .ToList();

            foreach (var type in allOptions)
            {
                var options = (IHttpClientOptions)Activator.CreateInstance(type);
                configuration.Bind(type.Name, options);

                var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Configure" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(type);

                configureMethod.Invoke(null, [ services, configuration.GetSection(type.Name) ]);

                services.AddHttpClient(type.Name, client =>
                {
                    client.BaseAddress = new Uri(options?.BaseUrl);
                    client.DefaultRequestHeaders.Add("user-agent", "AggregationApi/0.1");
                });
                var ifc = typeof(IHttpClientWrapper<>).MakeGenericType(type);
                var impl = typeof(HttpClientWrapper<>).MakeGenericType(type);

                services.Add(new ServiceDescriptor(ifc, impl, ServiceLifetime.Singleton));
            }
            return services;
        }
    }
}