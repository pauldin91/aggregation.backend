using Aggregation.Backend.Domain.Dtos.External.Air;
using Aggregation.Backend.Domain.Dtos.External.News;
using Aggregation.Backend.Domain.Dtos.External.StockMarket;

namespace Aggregation.Backend.Domain.Dtos.Aggregates
{
    public class AggregatedResponse
    {
        public ArticleDto? Article { get; set; }
        public CityDto? City { get; set; }
        public FeedDto? Feed { get; set; }
    }

}