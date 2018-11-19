using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using WhisperAPI.Models;
using WhisperAPI.Models.MegaGenial;
using WhisperAPI.Models.MLAPI;
using WhisperAPI.Models.NLPAPI;
using WhisperAPI.Models.Queries;
using WhisperAPI.Models.Search;
using WhisperAPI.Services.MLAPI.Facets;
using WhisperAPI.Services.NLPAPI;
using WhisperAPI.Services.Search;
[assembly: InternalsVisibleTo("WhisperAPI.Tests")]

namespace WhisperAPI.Services.Suggestions
{
    public class SuggestionsService : ISuggestionsService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ModuleQuestionsRepo _questionRepo = new ModuleQuestionsRepo();

        private static readonly ModuleDetector _moduleDetector = new ModuleDetector();

        private readonly IIndexSearch _indexSearch;

        private readonly INlpCall _nlpCall;

        private readonly IDocumentFacets _documentFacets;

        private readonly IFilterDocuments _filterDocuments;

        private readonly List<string> _irrelevantIntents;

        public SuggestionsService(
            IIndexSearch indexSearch,
            INlpCall nlpCall,
            IDocumentFacets documentFacets,
            IFilterDocuments filterDocuments,
            List<string> irrelevantIntents)
        {
            this._indexSearch = indexSearch;
            this._nlpCall = nlpCall;
            this._documentFacets = documentFacets;
            this._filterDocuments = filterDocuments;
            this._irrelevantIntents = irrelevantIntents ?? new List<string>();
        }

        public Suggestion GetNewSuggestion(ConversationContext conversationContext, SuggestionQuery query)
        {
            var documents = this.GetDocuments(conversationContext).ToList();
            conversationContext.LastNotFilteredDocuments = documents;
            conversationContext.FilterDocumentsParameters.Documents = documents.Select(x => x.Uri).ToList();
            return this.GetSuggestion(conversationContext, query);
        }

        public Suggestion GetLastSuggestion(ConversationContext conversationContext, SuggestionQuery query)
        {
            return this.GetSuggestion(conversationContext, query);
        }

        public IEnumerable<Document> GetDocuments(ConversationContext conversationContext)
        {
            var allRelevantQueries = string.Join(" ", conversationContext.SearchQueries.Where(x => x.Relevant).Select(m => m.ParsedQuery));

            if (string.IsNullOrEmpty(allRelevantQueries.Trim()))
            {
                return new List<Document>();
            }

            var coveoIndexDocuments = this.SearchCoveoIndex(allRelevantQueries, conversationContext.SuggestedDocuments.ToList());

            return this.FilterOutChosenSuggestions(coveoIndexDocuments, conversationContext.SearchQueries);
        }

        public void UpdateContextWithNewQuery(ConversationContext context, SearchQuery searchQuery)
        {
            searchQuery.Relevant = this.IsQueryRelevant(searchQuery);
            context.SearchQueries.Add(searchQuery);
        }

        public bool UpdateContextWithSelectedSuggestion(ConversationContext conversationContext, Guid selectQueryId)
        {
            Document document = conversationContext.SuggestedDocuments.ToList().Find(x => x.Id == selectQueryId);
            if (document != null)
            {
                conversationContext.SelectedSuggestedDocuments.Add(document);
                return true;
            }

            Question question = conversationContext.Questions.ToList().Find(x => x.Id == selectQueryId);
            if (question != null)
            {
                question.Status = QuestionStatus.Clicked;
                return true;
            }

            return false;
        }

        internal bool IsQueryRelevant(SearchQuery searchQuery)
        {
            var nlpAnalysis = this._nlpCall.GetNlpAnalysis(searchQuery.Query);
            searchQuery.ParsedQuery = nlpAnalysis.ParsedQuery;

            nlpAnalysis.Intents.ForEach(x => Log.Debug($"Intent - Name: {x.Name}, Confidence: {x.Confidence}"));
            nlpAnalysis.Entities.ForEach(x => Log.Debug($"Entity - Name: {x.Name}"));
            return this.IsIntentRelevant(nlpAnalysis);
        }

        internal IEnumerable<Document> FilterOutChosenSuggestions(
            IEnumerable<Document> coveoIndexDocuments,
            IEnumerable<SearchQuery> queriesList)
        {
            var queries = queriesList
                .Select(x => x.Query)
                .ToList();

            return coveoIndexDocuments.Where(x => !queries.Any(y => y.Contains(x.Uri)));
        }

        private static void AssociateKnownQuestionsWithId(ConversationContext conversationContext, List<Question> questions)
        {
            foreach (var question in questions)
            {
                var associatedQuestion = conversationContext.Questions.SingleOrDefault(contextQuestion => contextQuestion.Text.Equals(question.Text));
                question.Id = associatedQuestion?.Id ?? question.Id;
            }
        }

        private static IEnumerable<Question> FilterOutChosenQuestions(
            ConversationContext conversationContext,
            IEnumerable<Question> questions)
        {
            var questionsText = conversationContext.
                Questions.Where(question => question.Status != QuestionStatus.None && question.Status != QuestionStatus.Clicked)
                .Select(x => x.Text);

            return questions.Where(x => !questionsText.Any(y => y.Contains(x.Text)));
        }

        private static IEnumerable<Facet> GetActiveFacets(ConversationContext conversationContext)
        {
            return conversationContext.AnsweredQuestions.OfType<FacetQuestion>().Select(a => new Facet
            {
                Id = a.Id,
                Name = a.FacetName,
                Value = a.Answer
            }).ToList();
        }

        private static void UpdateContextWithNewSuggestions(ConversationContext context, List<Document> documents)
        {
            foreach (var document in documents)
            {
                context.SuggestedDocuments.Add(document);
            }
        }

        private static void UpdateContextWithNewQuestions(ConversationContext context, List<Question> questions)
        {
            context.LastSuggestedQuestions.Clear();
            foreach (var question in questions)
            {
                context.Questions.Add(question);
                context.LastSuggestedQuestions.Add(question);
            }
        }

        private IEnumerable<Question> GetQuestionsFromDocument(ConversationContext conversationContext, IEnumerable<Document> documents)
        {
            var questions = this._documentFacets.GetQuestions(documents.Select(x => x.Uri));
            AssociateKnownQuestionsWithId(conversationContext, questions.Cast<Question>().ToList());
            return FilterOutChosenQuestions(conversationContext, questions);
        }

        private List<Document> FilterDocumentsByFacet(ConversationContext conversationContext)
        {
            var filteredDocuments = this._filterDocuments.FilterDocumentsByFacets(conversationContext.FilterDocumentsParameters);
            return conversationContext.LastNotFilteredDocuments.Where(d => filteredDocuments.Contains(d.Uri)).ToList();
        }

        private Suggestion GetSuggestion(ConversationContext conversationContext, SuggestionQuery suggestionQuery)
        {
            var suggestion = new Suggestion
            {
                ActiveFacets = GetActiveFacets(conversationContext).ToList()
            };

            if (suggestion.ActiveFacets.Any())
            {
                var documents = this.FilterDocuments(conversationContext).Take(suggestionQuery.MaxDocuments).ToList();
                suggestion.Documents = documents;
            }
            else
            {
                suggestion.Documents = conversationContext.LastNotFilteredDocuments.Take(suggestionQuery.MaxDocuments).ToList();
            }

            if (suggestionQuery is SearchQuery)
            {
                suggestion.Questions = this.GenerateQuestions(conversationContext, (SearchQuery)suggestionQuery).Take(suggestionQuery.MaxQuestions).ToList();
            }

            return suggestion;
        }

        private IEnumerable<Document> FilterDocuments(ConversationContext conversationContext)
        {
            var documentsFiltered = this.FilterDocumentsByFacet(conversationContext);

            UpdateContextWithNewSuggestions(conversationContext, documentsFiltered);
            documentsFiltered.ForEach(x =>
                Log.Debug($"Id: {x.Id}, Title: {x.Title}, Uri: {x.Uri}, PrintableUri: {x.PrintableUri}, Summary: {x.Summary}"));

            return documentsFiltered;
        }

        private IEnumerable<QuestionToClient> GenerateQuestions(ConversationContext conversationContext, SearchQuery query)
        {
            var questions = new List<Question>();
            var moduleList = _moduleDetector.DetectModuleList(query.Query);
            foreach (var (module, score) in moduleList)
            {
                questions.AddRange(_questionRepo.GetQuestions(module));
            }

            var questionsToClient = questions.Select(QuestionToClient.FromQuestion).ToList();

            UpdateContextWithNewQuestions(conversationContext, questions);
            questions.ForEach(x => Log.Debug($"Id: {x.Id}, Text: {x.Text}"));

            return questionsToClient;
        }

        private IEnumerable<Document> SearchCoveoIndex(string query, List<Document> suggestedDocuments)
        {
            ISearchResult searchResult = this._indexSearch.Search(query);
            var documents = new List<Document>();

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
                    Document document = suggestedDocuments.Find(x => x.Uri == result.Uri) ?? new Document(result);
                    documents.Add(document);
                }
            }

            return documents;
        }

        private bool IsIntentRelevant(NlpAnalysis nlpAnalysis)
        {
            var mostConfidentIntent = nlpAnalysis.Intents.OrderByDescending(x => x.Confidence).First();
            return !this._irrelevantIntents.Any(x => Regex.IsMatch(mostConfidentIntent.Name, this.WildCardToRegularExpression(x)));
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

        private string WildCardToRegularExpression(string value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }
    }
}
