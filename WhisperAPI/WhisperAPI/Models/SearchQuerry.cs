using Newtonsoft.Json;

namespace WhisperAPI.Models
{
    public class SearchQuerry
    {
        [JsonProperty("chatkey")]
        public string ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }
    }
}