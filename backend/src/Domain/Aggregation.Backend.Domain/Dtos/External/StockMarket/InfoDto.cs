using Newtonsoft.Json;

namespace Aggregation.Backend.Domain.Dtos.External.StockMarket
{
    public class InfoDto
    {
        [JsonProperty("Information")]
        public string Information { get; set; }
    }
}