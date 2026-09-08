using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Infrastructure.Cache;
using Polly;
using System.Diagnostics;
using System.Net;

namespace Aggregation.Backend.Infrastructure.Services
{
    public class HttpClientWrapper<T>(IHttpClientFactory httpClientFactory, ExternalApiRequestTimingCache externalApiRequestTimingCache) : IHttpClientWrapper<T>
        where T : IHttpClientOptions, new()
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient(typeof(T).Name);
        private static AsyncPolicy<HttpResponseMessage> combinedPolicy = FallbackPolicy();

        public async Task<string> GetAsync(string uri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            var stopwatch = Stopwatch.StartNew();

            var response = await combinedPolicy.ExecuteAsync(() => _httpClient.SendAsync(request, cancellationToken));

            stopwatch.Stop();
            externalApiRequestTimingCache.Record(_httpClient?.BaseAddress?.ToString(), stopwatch.ElapsedMilliseconds);

            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return content;
            }
            return string.Empty;
        }

        public static AsyncPolicy<HttpResponseMessage> FallbackPolicy()
        {
            return Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(r => !r.IsSuccessStatusCode)
                    .FallbackAsync(
                            new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new StringContent("")
                            }
                    );
        }
    }
}