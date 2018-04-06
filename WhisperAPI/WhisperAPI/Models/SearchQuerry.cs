using System;
using Newtonsoft.Json;

namespace WhisperAPI.Models
{
    public class SearchQuerry
    {
        [JsonProperty("chatkey")]
        public Guid ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }
    }
}