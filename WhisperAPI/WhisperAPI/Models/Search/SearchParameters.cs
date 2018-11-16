using Newtonsoft.Json;

namespace WhisperAPI.Models.Search
{
    public class SearchParameters
    {
        [JsonProperty(PropertyName = "q")]
        public string Q { get; set; }

        [JsonProperty(PropertyName = "numberOfResults")]
        public int NumberOfResults { get; set; }
    }
}
