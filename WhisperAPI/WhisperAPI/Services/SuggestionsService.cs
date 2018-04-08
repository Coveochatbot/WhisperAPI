using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using WhisperAPI.Models;
using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Services
{
    public class SuggestionsService : ISuggestionsService
    {
        private readonly IIndexSearch _indexSearch;

        private readonly INlpCall _nlpCall;

        private readonly List<string> _irrelevantsIntents;

        public SuggestionsService(IIndexSearch indexSearch, INlpCall nlpCall, List<string> irrelevantsIntents)
        {
            this._indexSearch = indexSearch;
            this._nlpCall = nlpCall;
            this._irrelevantsIntents = irrelevantsIntents;
        }

        public IEnumerable<SuggestedDocument> GetSuggestions(List<SearchQuerry> querriesList)
        {
            IEnumerable<SuggestedDocument> suggestedDocuments = new List<SuggestedDocument>();
            var allMessages = string.Join(" ", querriesList.Select(m => m.Querry));
            var nlpAnalysis = this._nlpCall.GetNlpAnalysis(querriesList.LastOrDefault()?.Querry);

            // TODO: filter out the querry from lq when the irrelevantsIntents matches the ones in the settings and when persistance will be done
            if (this.IsIntentRelevant(nlpAnalysis))
            {
                var coveoIndexDocuments = this.SearchCoveoIndex(allMessages);
                suggestedDocuments = this.FilterOutChoosenSuggestions(coveoIndexDocuments, querriesList);
            }

            return suggestedDocuments;
        }

        private IEnumerable<SuggestedDocument> FilterOutChoosenSuggestions(
            IEnumerable<SuggestedDocument> coveoIndexDocuments,
            IEnumerable<SearchQuerry> querriesList)
        {
            var agentQuerries = querriesList
                .Where(x => x.Type == SearchQuerry.MessageType.Agent)
                .Select(x => x.Querry)
                .ToList();
            return coveoIndexDocuments.Where(x => !agentQuerries.Contains(x.Title));
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
            var mostConfidentIntent = nlpAnalysis.Intents.OrderByDescending(x => x.Confidence).First();

            return !this._irrelevantsIntents.Contains(mostConfidentIntent.Name);
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
