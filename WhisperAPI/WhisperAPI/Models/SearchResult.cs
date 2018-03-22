using System.Collections.Generic;
using Newtonsoft.Json;

namespace WhisperAPI.Models
{
    public class SearchResult : ISearchResult
    {
        [JsonProperty("totalCount")]
        private int _nbrResults;

        [JsonProperty("results")]
        private IEnumerable<SearchResultElement> _elements;

        public int NbrElements { get => this._nbrResults; set => this._nbrResults = value; }

        public IEnumerable<ISearchResultElement> Elements { get => this._elements; set => this._elements = (IEnumerable<SearchResultElement>)value; }
    }
}
