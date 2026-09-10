namespace Aggregation.Backend.Application.Interfaces
{
    public interface IHttpClientOptions
    {
        string BaseUrl { get; set; }

    }
    public interface IHttpClientApiKeyOptions : IHttpClientOptions
    {
        string ApiKey { get; set; }
        string ListUri { get; set; }
    }
    public interface IHttpOAuth2ClientOptions : IHttpClientOptions
    {
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string Scope { get; set; }
        string TokenUrl { get; set; }
    }
}