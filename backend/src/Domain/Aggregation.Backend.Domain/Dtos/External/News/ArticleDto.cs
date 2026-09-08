using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.News
{
    public class ArticleDto : IMappedDto
    {
        [JsonProperty("source")]
        public SourceDto Source { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("urlToImage")]
        public string UrlToImage { get; set; }

        [JsonProperty("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        public Dictionary<string, object> ToMap()
        {
            var map = new Dictionary<string, object> {
                {nameof(Author),Author },
                {nameof(Title),Title },
                {nameof(Description),Description },
                {nameof(Url),Url },
                {nameof(UrlToImage),UrlToImage },
                {nameof(PublishedAt),PublishedAt},
                {nameof(Content),Content },
            };
            if (Source is not null)
            {
                foreach (var item in Source.ToMap())
                {
                    map.Add(nameof(Source) + "." + item.Key, item.Value);
                }
            }
            return map;
        }
    }
}