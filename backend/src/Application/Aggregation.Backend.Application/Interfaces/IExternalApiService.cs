using Aggregation.Backend.Domain.Dtos.Aggregates;

namespace Aggregation.Backend.Application.Interfaces
{
    public interface IExternalApiService
    {
        Task<IList<Dictionary<string, object>>> ListAsync(string category,CancellationToken cancellationToken);
    }
}