using Newtonsoft.Json;

namespace WhisperAPI.Models
{

    public class SearchQuerry
    {
        public enum MessageType
        {
            Error = 0,
            Chasitor = 1,
            Agent = 2
        }

        [JsonProperty("chatkey")]
        public string ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }

        [JsonProperty("type")]
        public MessageType Type { get; set; }
    }
}