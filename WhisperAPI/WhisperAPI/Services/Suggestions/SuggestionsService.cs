﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WhisperAPI.Models;
using WhisperAPI.Models.MLAPI;
using WhisperAPI.Models.NLPAPI;
using WhisperAPI.Models.Queries;
using WhisperAPI.Models.Search;
using WhisperAPI.Services.MLAPI.Facets;
using WhisperAPI.Services.NLPAPI;
using WhisperAPI.Services.Search;

namespace WhisperAPI.Services.Suggestions
{
    public class SuggestionsService : ISuggestionsService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            this._irrelevantIntents = irrelevantIntents;
        }

        public IEnumerable<SuggestedDocument> GetSuggestedDocuments(ConversationContext conversationContext)
        {
            var allRelevantQueries = string.Join(" ", conversationContext.SearchQueries.Where(x => x.Relevant).Select(m => m.Query));

            if (string.IsNullOrEmpty(allRelevantQueries.Trim()))
            {
                return new List<SuggestedDocument>();
            }

            var coveoIndexDocuments = this.SearchCoveoIndex(allRelevantQueries);

            return this.FilterOutChosenSuggestions(coveoIndexDocuments, conversationContext.SearchQueries);
        }

        public IEnumerable<Question> GetQuestionsFromDocument(ConversationContext conversationContext, IEnumerable<SuggestedDocument> suggestedDocuments)
        {
            var questions = this._documentFacets.GetQuestions(suggestedDocuments.Select(x => x.Uri));
            this.AssociateKnownQuestionsWithId(conversationContext, questions.Cast<Question>().ToList());
            return FilterOutChosenQuestions(conversationContext, questions);
        }

        public List<string> FilterDocumentsByFacet(FilterDocumentsParameters parameters)
        {
            var documents = this._filterDocuments.FilterDocumentsByFacets(parameters);
            return documents;
        }

        public void AssociateKnownQuestionsWithId(ConversationContext conversationContext, List<Question> questions)
        {
            foreach (var question in questions)
            {
                var associatedQuestion = conversationContext.Questions.Where(contextQuestion => contextQuestion.Text.Equals(question.Text)).SingleOrDefault();
                question.Id = associatedQuestion?.Id ?? question.Id;
            }
        }

        public void UpdateContextWithNewQuery(ConversationContext context, SearchQuery searchQuery)
        {
            searchQuery.Relevant = this.IsQueryRelevant(searchQuery);
            context.SearchQueries.Add(searchQuery);
        }

        public void UpdateContextWithNewSuggestions(ConversationContext context, List<SuggestedDocument> suggestedDocuments)
        {
            context.LastSuggestedDocuments.Clear();
            foreach (var suggestedDocument in suggestedDocuments)
            {
                context.SuggestedDocuments.Add(suggestedDocument);
                context.LastSuggestedDocuments.Add(suggestedDocument);
            }
        }

        public void UpdateContextWithNewQuestions(ConversationContext context, List<Question> questions)
        {
            context.LastSuggestedQuestions.Clear();
            foreach (var question in questions)
            {
                context.Questions.Add(question);
                context.LastSuggestedQuestions.Add(question);
            }
        }

        public bool UpdateContextWithSelectedSuggestion(ConversationContext conversationContext, Guid selectQueryId)
        {
            SuggestedDocument suggestedDocument = conversationContext.SuggestedDocuments.ToList().Find(x => x.Id == selectQueryId);
            if (suggestedDocument != null)
            {
                conversationContext.SelectedSuggestedDocuments.Add(suggestedDocument);
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

        public bool IsQueryRelevant(SearchQuery searchQuery)
        {
            var nlpAnalysis = this._nlpCall.GetNlpAnalysis(searchQuery.Query);

            nlpAnalysis.Intents.ForEach(x => Log.Debug($"Intent - Name: {x.Name}, Confidence: {x.Confidence}"));
            nlpAnalysis.Entities.ForEach(x => Log.Debug($"Entity - Name: {x.Name}"));
            return this.IsIntentRelevant(nlpAnalysis);
        }

        public IEnumerable<SuggestedDocument> FilterOutChosenSuggestions(
            IEnumerable<SuggestedDocument> coveoIndexDocuments,
            IEnumerable<SearchQuery> queriesList)
        {
            var queries = queriesList
                .Select(x => x.Query)
                .ToList();

            return coveoIndexDocuments.Where(x => !queries.Any(y => y.Contains(x.Uri)));
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
