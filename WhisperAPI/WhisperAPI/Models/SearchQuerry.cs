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

        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "ChatKey is required")]
        [JsonProperty("chatkey")]
        public Guid? ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [JsonProperty("type")]
        public MessageType? Type { get; set; }

        [JsonIgnore]
        public bool Relevant { get; set; }
    }
}