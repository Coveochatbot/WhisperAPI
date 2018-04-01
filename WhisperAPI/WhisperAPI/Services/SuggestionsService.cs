using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Services
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly IIndexSearch _indexSearch;

        private readonly INlpCall _nlpCall;

        private readonly List<string> _intents;

        public SuggestionsService(IIndexSearch indexSearch, INlpCall nlpCall, List<string> intents)
        {
            this._indexSearch = indexSearch;
            this._nlpCall = nlpCall;
            this._intents = intents;
        }

        public IEnumerable<SuggestedDocument> GetSuggestions(string querry)
        {
            var nlpAnalysis = this._nlpCall.GetNlpAnalyses(querry);

            // TODO: filter out the querry from lq when the intents matches the ones in the settings and when persistance will be done
            if (this.IsIntentRelevant(nlpAnalysis))
            {
                return this.SearchCoveoIndex(querry);
            }

            return new List<SuggestedDocument>();
        }

        private IEnumerable<SuggestedDocument> SearchCoveoIndex(string querry)
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
                if (this.IsElementValid(result))
                {
                    documents.Add(new SuggestedDocument(result));
                }
            }

            return documents;
        }

        private bool IsIntentRelevant(NlpAnalysis nlpAnalysis)
        {
            foreach (var intent in nlpAnalysis.Intents)
            {
                foreach (var intentToFilter in this._intents)
                {
                    if (intentToFilter == intent.Name)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsElementValid(ISearchResultElement result)
        {
            if (result?.Title == null || result?.Uri == null || result?.PrintableUri == null)
            {
                // error null attributes
                return false;
            }

            return true;
        }
    }
}
