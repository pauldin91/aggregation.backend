using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.Air
{
    public class NameOnlyDto
    {
        [JsonProperty("city")]
        public string City { get; set; }
    }

    public class DataDto : NameOnlyDto, IMappedDto
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("location")]
        public LocationDto Location { get; set; }

        [JsonProperty("current")]
        public CurrentDto Current { get; set; }

        public Dictionary<string, object> ToMap()
        {
            var map = new Dictionary<string, object>
            {
                { nameof(City), City },
                { nameof(State), State },
                { nameof(Country), Country },
            };
            if (Location is not null)
            {
                foreach (var item in Location.ToMap())
                {
                    map.Add(nameof(Location) + "." + item.Key, item.Value);
                }
            }

            if (Current is not null)
            {
                foreach (var item in Current.ToMap())
                {
                    map.Add(nameof(Current) + "." + item.Key, item.Value);
                }
            }
            return map;
        }
    }
}