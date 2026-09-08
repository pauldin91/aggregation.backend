namespace Aggregation.Backend.Application.Interfaces
{
    public interface IHttpClientWrapper<T>
        where T : IHttpClientOptions, new()
    {
        Task<string> GetAsync(string uri,CancellationToken cancellationToken);
    }
}