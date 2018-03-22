using Newtonsoft.Json;

namespace WhisperAPI.Models
{
    public class SearchQuerry
    {
        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }
    }
}