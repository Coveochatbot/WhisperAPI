using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WhisperAPI.Models
{
    public class SearchQuerry
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ChatKey is required")]
        [JsonProperty("chatkey")]
        public Guid ChatKey { get; set; }

        [JsonProperty("querry")]
        public string Querry { get; set; }
    }
}