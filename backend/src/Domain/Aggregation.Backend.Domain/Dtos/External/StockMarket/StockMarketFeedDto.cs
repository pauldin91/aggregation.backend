using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.StockMarket
{

    public class StockMarketFeedDto
    {
        [JsonProperty("items")]
        public string Items { get; set; }

        [JsonProperty("sentiment_score_definition")]
        public string SentimentScoreDefinition { get; set; }

        [JsonProperty("relevance_score_definition")]
        public string RelevanceScoreDefinition { get; set; }

        [JsonProperty("feed")]
        public List<FeedDto> Feed { get; set; }
    }


}