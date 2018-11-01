using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace WhisperAPI.Models.Queries
{
    public class SuggestionQuery : Query
    {
        [Required(ErrorMessage = "MaxDocuments is required")]
        [Range(0, int.MaxValue)]
        [JsonProperty("maxDocuments")]
        public int MaxDocuments { get; set; }

        [Required(ErrorMessage = "maxQuestions is required")]
        [Range(0, int.MaxValue)]
        [JsonProperty("maxQuestions")]
        public int MaxQuestions { get; set; }
    }
}
