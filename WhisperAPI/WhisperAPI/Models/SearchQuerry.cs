using System;
using System.ComponentModel.DataAnnotations;
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "ChatKey is required")]
        [JsonProperty("chatkey")]
        public Guid ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }

        [JsonProperty("type")]
        public MessageType Type { get; set; }
    }
}