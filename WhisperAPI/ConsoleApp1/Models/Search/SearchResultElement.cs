using System.Collections.Generic;
using Newtonsoft.Json;

namespace ConsoleApp1.Models.Search
{
    public class SearchResultElement : ISearchResultElement
    {
        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("clickUri")]
        public string clickUri { get; set; }

        [JsonProperty("printableUri")]
        public string printableUri { get; set; }

        [JsonProperty("summary")]
        public string summary { get; set; }

        [JsonProperty("excerpt")]
        public string excerpt { get; set; }

        [JsonProperty("uri")]
        public string uri { get; set; }

        [JsonProperty("uniqueId")]
        public string uniqueId { get; set; }

        [JsonProperty("raw")]
        public RawInformation rawInformation { get; set; }
    }
}
