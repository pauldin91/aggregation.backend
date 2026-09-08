using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.StockMarket
{
    public class TickerSentimentDto : IMappedDto
    {
        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("relevance_score")]
        public string RelevanceScore { get; set; }

        [JsonProperty("ticker_sentiment_score")]
        public string TickerSentimentScore { get; set; }

        [JsonProperty("ticker_sentiment_label")]
        public string TickerSentimentLabel { get; set; }

        public Dictionary<string, object> ToMap() => new()
        {
            { nameof(Ticker),Ticker},
            { nameof(RelevanceScore),RelevanceScore},
            { nameof(TickerSentimentScore),TickerSentimentScore},
            { nameof(TickerSentimentLabel),TickerSentimentLabel},
        };
    }
}