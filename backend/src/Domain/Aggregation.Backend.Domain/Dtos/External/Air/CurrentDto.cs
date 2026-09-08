using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.Air
{
    public class CurrentDto : IMappedDto
    {
        [JsonProperty("pollution")]
        public PollutionDto Pollution { get; set; }

        [JsonProperty("weather")]
        public WeatherDto Weather { get; set; }

        public Dictionary<string, object> ToMap()
        {
            var map = new Dictionary<string, object>();
            if (Pollution is not null)
            {
                foreach (var item in Pollution.ToMap())
                {
                    map.Add(nameof(Pollution) + "." + item.Key, item.Value);
                }
            }
            if (Weather is not null)
            {
                foreach (var item in Weather.ToMap())
                {
                    map.Add(nameof(Weather) + "." + item.Key, item.Value);
                }
            }

            return map;
        }
    }
}