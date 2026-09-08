using Aggregation.Backend.Application.Interfaces;
using Aggregation.Backend.Domain.Dtos.External.StockMarket;
using Aggregation.Backend.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aggregation.Backend.Infrastructure.Services
{
    public class StockMarketFeedService(IHttpClientWrapper<StockMarketFeedOptions> httpClientWrapper, IOptions<StockMarketFeedOptions> options) : IStockMarketFeedService
    {

        public async Task<IList<Dictionary<string, object>>> ListAsync(string category, CancellationToken cancellationToken)
        {
            var queryString = $"function=NEWS_SENTIMENT&tickers={category}&apikey={options.Value.ApiKey}";
            var relativePath = string.Join("?", options.Value.ListUri, queryString);
            var response = await httpClientWrapper.GetAsync(relativePath, cancellationToken);
            var infoDto = JsonConvert.DeserializeObject<InfoDto>(response);
            if (string.IsNullOrEmpty(response) || ( infoDto?.Information is not null) )
            {
                return new List<Dictionary<string, object>>();
            }

            var stockFeed = JsonConvert.DeserializeObject<StockMarketFeedDto>(response);
                var result = stockFeed?.Feed.Select(s => s.ToMap()).ToList() ?? new List<Dictionary<string, object>>();
                return result;
           

        }
    }
}