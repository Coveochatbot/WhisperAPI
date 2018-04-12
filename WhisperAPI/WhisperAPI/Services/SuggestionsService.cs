using System.Collections.Generic;
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

        public IEnumerable<SuggestedDocument> GetSuggestions(ConversationContext conversationContext)
        {
            var allRelevantQueries = string.Join(" ", conversationContext.SearchQueries.Where(x => x.Relevant).Select(m => m.Query));

            if (string.IsNullOrEmpty(allRelevantQueries.Trim()))
            {
                return new List<SuggestedDocument>();
            }

            var coveoIndexDocuments = this.SearchCoveoIndex(allRelevantQueries);

            // TODO: Send 5 most relevant results
            return this.FilterOutChosenSuggestions(coveoIndexDocuments, conversationContext.SearchQueries).Take(5);
        }

        public void UpdateContextWithNewQuery(ConversationContext context, SearchQuery searchQuery)
        {
            searchQuery.Relevant = this.IsQueryRelevant(searchQuery);
            context.SearchQueries.Add(searchQuery);
        }

        public void UpdateContextWithNewSuggestions(ConversationContext context, List<SuggestedDocument> suggestedDocuments)
        {
            foreach (var suggestedDocument in suggestedDocuments)
            {
                context.SuggestedDocuments.Add(suggestedDocument);
            }
        }

        public bool IsQueryRelevant(SearchQuery searchQuery)
        {
            var nlpAnalysis = this._nlpCall.GetNlpAnalysis(searchQuery.Query);
            return this.IsIntentRelevant(nlpAnalysis);
        }

        public IEnumerable<SuggestedDocument> FilterOutChosenSuggestions(
            IEnumerable<SuggestedDocument> coveoIndexDocuments,
            IEnumerable<SearchQuery> queriesList)
        {
            var agentQueries = queriesList
                .Where(x => x.Type == SearchQuery.MessageType.Agent)
                .Select(x => x.Query)
                .ToList();

            return coveoIndexDocuments.Where(x => !agentQueries.Contains(x.Uri));
        }

        private IEnumerable<SuggestedDocument> SearchCoveoIndex(string query)
        {
            ISearchResult searchResult = this._indexSearch.Search(query);
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
