using Newtonsoft.Json;

namespace WhisperAPI.Models
{

    public class SearchQuerry
    {
        public enum MessageType
        {
            Customer = 0,
            Agent = 1
        }

        [JsonProperty("chatkey")]
        public string ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }

        [JsonProperty("type")]
        public MessageType Type { get; set; }
    }
}