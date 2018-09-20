using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Models
{
    public class SearchQuery : Query
    {
        public enum MessageType
        {
            Customer = 0,
            Agent = 1
        }

        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("query")]
        public string Query { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [JsonProperty("type")]
        public MessageType? Type { get; set; }

        [JsonIgnore]
        public bool Relevant { get; set; }
    }
}