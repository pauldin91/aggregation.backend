using Aggregation.Backend.Application.Interfaces;

namespace Aggregation.Backend.Infrastructure.Options
{
    public class ExternalIdProviderOptions : IHttpOAuth2ClientOptions
    {
        public string Scope { get ; set ; }
        public string TokenUrl { get ; set ; }
        public string BaseUrl { get ; set ; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}