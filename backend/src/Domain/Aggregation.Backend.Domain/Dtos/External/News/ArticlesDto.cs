using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.News
{

    public class ArticlesDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("articles")]
        public List<ArticleDto> Articles { get; set; }
    }
}