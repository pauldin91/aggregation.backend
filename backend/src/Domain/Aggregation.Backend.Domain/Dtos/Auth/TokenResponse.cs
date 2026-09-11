using System.Text.Json.Serialization;

namespace Aggregation.Backend.Domain.Dtos.Auth
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public DateTime Exp { get; set; }
    }
}