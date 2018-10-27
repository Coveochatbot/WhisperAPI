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

        [Required(ErrorMessage = "MaxDocuments is required")]
        [JsonProperty("maxDocuments")]
        public int MaxDocuments { get; set; } = 10;

        [Required(ErrorMessage = "maxQuestions is required")]
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; } = 10;
    }
}