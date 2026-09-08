using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.Air
{
    public class CitiesDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public List<NameOnlyDto> Cities { get; set; }
    }
}