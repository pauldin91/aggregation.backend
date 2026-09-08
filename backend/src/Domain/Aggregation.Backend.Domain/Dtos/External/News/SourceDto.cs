using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.News
{
    public class SourceDto : IMappedDto
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public Dictionary<string, object> ToMap()
        {
            return new Dictionary<string, object> {
                {nameof(Id),Id },
                {nameof(Name),Name },
            };
        }
    }
}