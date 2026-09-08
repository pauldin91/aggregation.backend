using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.StockMarket
{
    public class TopicDto : IMappedDto
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("relevance_score")]
        public string RelevanceScore { get; set; }

        public Dictionary<string, object> ToMap() => new() { 
            {nameof(Topic),Topic } ,
            {nameof(RelevanceScore),RelevanceScore } 
        };
    }
}