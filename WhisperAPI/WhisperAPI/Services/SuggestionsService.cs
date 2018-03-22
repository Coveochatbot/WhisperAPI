using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly IIndexSearch _indexSearch;

        public SuggestionsService(IIndexSearch indexSearch)
        {
            this._indexSearch = indexSearch;
        }

        public IEnumerable<SuggestedDocument> GetSuggestion(string querry)
        {
            ISearchResult searchResult = this._indexSearch.Search(querry);

            var documents = new List<SuggestedDocument>();
            foreach (var result in searchResult.Elements)
            {
                documents.Add(new SuggestedDocument(result));
            }

            return documents;
        }
    }
}
