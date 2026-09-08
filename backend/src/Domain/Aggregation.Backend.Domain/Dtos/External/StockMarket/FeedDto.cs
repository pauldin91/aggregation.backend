using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.StockMarket
{
    public class FeedDto : IMappedDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("time_published")]
        public string TimePublished { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("banner_image")]
        public string BannerImage { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("category_within_source")]
        public string CategoryWithinSource { get; set; }

        [JsonProperty("source_domain")]
        public string SourceDomain { get; set; }

        [JsonProperty("overall_sentiment_score")]
        public double OverallSentimentScore { get; set; }

        [JsonProperty("overall_sentiment_label")]
        public string OverallSentimentLabel { get; set; }

        [JsonProperty("authors")]
        public List<string> Authors { get; set; }

        [JsonProperty("topics")]
        public List<TopicDto> Topics { get; set; }

        [JsonProperty("ticker_sentiment")]
        public List<TickerSentimentDto> TickerSentiment { get; set; }

        public Dictionary<string, object> ToMap()
        {
            var result = new Dictionary<string, object>
            {
                { nameof(Title), Title },
                { nameof(Url), Url },
                { nameof(TimePublished), TimePublished },

                { nameof(Summary), Summary },
                { nameof(BannerImage), BannerImage },
                { nameof(Source), Source },
                { nameof(CategoryWithinSource), CategoryWithinSource },
                { nameof(SourceDomain), SourceDomain },
                { nameof(OverallSentimentScore), OverallSentimentScore },
                { nameof(OverallSentimentLabel), OverallSentimentLabel },
            };

            if (Authors is not null && Authors.Count > 0)
            {
                result.Add(nameof(Authors), Authors);
            }
            if (Topics is not null && Topics.Count > 0)
            {
                result.Add(nameof(Topics), Topics.Select(s => s.ToMap()).ToList());
            }
            if (TickerSentiment is not null && TickerSentiment.Count > 0)
            {
                result.Add(nameof(TickerSentiment), TickerSentiment.Select(s => s.ToMap()).ToList());
            }
            return result;
        }
    }
}