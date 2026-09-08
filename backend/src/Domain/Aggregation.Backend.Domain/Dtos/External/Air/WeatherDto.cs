using Aggregation.Backend.Domain.Dtos.Interfaces;
using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.Air
{
    public class WeatherDto : IMappedDto
    {
        [JsonProperty("ts")]
        public DateTime Ts { get; set; }

        [JsonProperty("ic")]
        public string Ic { get; set; }

        [JsonProperty("hu")]
        public int Hu { get; set; }

        [JsonProperty("pr")]
        public int Pr { get; set; }

        [JsonProperty("tp")]
        public int Tp { get; set; }

        [JsonProperty("wd")]
        public int Wd { get; set; }

        [JsonProperty("ws")]
        public double Ws { get; set; }

        [JsonProperty("heat_index")]
        public int HeatIndex { get; set; }

        public Dictionary<string, object> ToMap()
        {
            return new Dictionary<string, object> {
                { nameof(Ts),Ts},
                { nameof(Ic),Ic},
                { nameof(Hu),Hu},
                { nameof(Pr),Pr},
                { nameof(Tp),Tp},
                { nameof(Wd),Wd},
                { nameof(Ws),Ws},
                { nameof(HeatIndex),HeatIndex},
            };

        }
    }
}