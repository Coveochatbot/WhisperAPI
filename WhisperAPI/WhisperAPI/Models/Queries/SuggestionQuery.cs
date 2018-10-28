using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WhisperAPI.Models.Queries
{
    public class SuggestionQuery : Query
    {
        [Required(ErrorMessage = "MaxDocuments is required")]
        [JsonProperty("maxDocuments")]
        public int MaxDocuments { get; set; }

        [Required(ErrorMessage = "maxQuestions is required")]
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; }
    }
}
