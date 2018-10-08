using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WhisperAPI.Models.Queries
{
    public class Query
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "ChatKey is required")]
        [JsonProperty("chatkey")]
        public Guid? ChatKey { get; set; }
    }
}