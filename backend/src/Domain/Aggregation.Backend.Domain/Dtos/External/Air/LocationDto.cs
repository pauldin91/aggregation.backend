using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.Air
{
    public class LocationDto : IMappedDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }

        public Dictionary<string, object> ToMap()
        {
            return  new Dictionary<string, object> {
                { nameof(Type), Type },
                { nameof(Coordinates), Coordinates},
            };

        }
    }
}