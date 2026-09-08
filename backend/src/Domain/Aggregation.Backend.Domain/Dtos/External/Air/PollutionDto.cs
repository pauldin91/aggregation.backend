using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace Aggregation.Backend.Domain.Dtos.External.Air
{
    public class PollutionDto : IMappedDto
    {
        [JsonProperty("ts")]
        public DateTime Ts { get; set; }

        [JsonProperty("aqius")]
        public int Aqius { get; set; }

        [JsonProperty("mainus")]
        public string Mainus { get; set; }

        [JsonProperty("aqicn")]
        public int Aqicn { get; set; }

        [JsonProperty("maincn")]
        public string Maincn { get; set; }

        public Dictionary<string, object> ToMap()
        {
            return  new Dictionary<string, object> {

                { nameof(Ts), Ts },
                { nameof(Aqius), Aqius },
                { nameof(Mainus), Mainus },
                { nameof(Aqicn), Aqicn},
                { nameof(Maincn), Maincn },
            };

        }
    }
}