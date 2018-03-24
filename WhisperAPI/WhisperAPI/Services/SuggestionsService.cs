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

            if (searchResult == null)
            {
                // error null result
                return documents;
            }

            if (searchResult.Elements == null)
            {
                // error null result elements
                return documents;
            }

            foreach (var result in searchResult.Elements)
            {
                if (!this.IsElementValid(result))
                {
                    continue;
                }

                documents.Add(new SuggestedDocument(result));
            }

            return documents;
        }

        private bool IsElementValid(ISearchResultElement result)
        {
            if (result == null)
            {
                // error null result
                return false;
            }

            if (result.Title == null || result.Uri == null || result.PrintableUri == null)
            {
                // error null attributes
                return false;
            }

            return true;
        }
    }
}
