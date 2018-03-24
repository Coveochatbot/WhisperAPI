using System.Collections.Generic;
using Newtonsoft.Json;

namespace WhisperAPI.Models
{
    public class SearchResult : ISearchResult
    {
        [JsonProperty("totalCount")]
        public int NbrElements { get; set; }

        [JsonProperty("results")]
        public IEnumerable<ISearchResultElement> Elements { get; set; }
    }
}
