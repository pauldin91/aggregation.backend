using Aggregation.Backend.Domain.Dtos.Aggregates;
using MediatR;

namespace Aggregation.Backend.Application.Features.Aggregates
{
    public record AggregatesQuery(string Category, string? FilterBy,  string? SortBy, bool Asc = true) : IRequest<IList<Dictionary<string, object>>>;
}