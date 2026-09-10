using Aggregation.Backend.Application.Interfaces;

namespace Aggregation.Backend.Infrastructure.Options
{
    public class AirPollutionOptions : IHttpClientApiKeyOptions
    {
        public string BaseUrl { get; set; }
        public string ListUri { get; set; }
        public string ApiKey { get; set; }
        public string GetUri { get; set; }

    }
}