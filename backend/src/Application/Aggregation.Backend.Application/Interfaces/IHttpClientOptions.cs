namespace Aggregation.Backend.Application.Interfaces
{
    public interface IHttpClientOptions
    {
        string BaseUrl { get; set; }
        string ListUri { get; set; }

        string ApiKey { get; set; }
    }
}