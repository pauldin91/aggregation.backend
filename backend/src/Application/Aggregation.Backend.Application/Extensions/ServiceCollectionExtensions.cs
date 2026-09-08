using Aggregation.Backend.Application.Features.Aggregates;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aggregation.Backend.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationExtensions(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AggregatesQueryHandler).Assembly));
            return services;
        }
    }
}