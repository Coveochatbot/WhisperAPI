using System;
using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;
using WhisperAPI.Services.Context;
using WhisperAPI.Services.Questions;
using WhisperAPI.Services.Suggestions;

namespace WhisperAPI.Controllers
{
    [Route("/Whisper/[Controller]")]
    public class SuggestionsController : ContextController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISuggestionsService _suggestionsService;

        private readonly IQuestionsService _questionsService;

        public SuggestionsController(ISuggestionsService suggestionsService, IQuestionsService questionsService, IContexts contexts)
            : base(contexts)
        {
            this._suggestionsService = suggestionsService;
            this._questionsService = questionsService;
        }

        [HttpPost]
        public IActionResult GetSuggestions([FromBody] SearchQuery searchQuery)
        {
            this._suggestionsService.UpdateContextWithNewQuery(this.ConversationContext, searchQuery);
            this._questionsService.DetectAnswer(this.ConversationContext, searchQuery);
            bool questionAskedDetected = this._questionsService.DetectQuestionAsked(this.ConversationContext, searchQuery);

            if (questionAskedDetected)
            {
                searchQuery.Relevant = false;
            }

<<<<<<< HEAD
            var suggestion = this._suggestionsService.GetNewSuggestion(this.ConversationContext, searchQuery);

            LogSuggestion(suggestion);
=======
            var suggestion = this._suggestionsService.GetSuggestion(this.ConversationContext, searchQuery);
>>>>>>> 5c832620657361ca3667d25bc90117f52030772f
            return this.Ok(suggestion);
        }

        [HttpGet]
        public IActionResult GetSuggestions(Query query)
        {
            Log.Debug($"Query: {query}");

<<<<<<< HEAD
            var suggestion = this._suggestionsService.GetLastSuggestion(this.ConversationContext, query);

            LogSuggestion(suggestion);

=======
            var suggestion = new Suggestion()
            {
                SuggestedDocuments = this.ConversationContext.LastSuggestedDocuments.Take(query.MaxDocuments).ToList(),
                Questions = this.ConversationContext.LastSuggestedQuestions.Select(QuestionToClient.FromQuestion).Take(query.MaxDocuments).ToList(),
                ActiveFacets = mustHaveFacets
            };

            suggestion.SuggestedDocuments.ForEach(x => Log.Debug($"Title: {x.Title}, Uri: {x.Uri}, PrintableUri: {x.PrintableUri}, Summary: {x.Summary}"));
            suggestion.Questions.ForEach(x => Log.Debug($"Id: {x.Id}, Text: {x.Text}"));
            suggestion.ActiveFacets.ForEach(x => Log.Debug($"Id: {x.Id}, Name: {x.Name}, Value: {x.Value}"));
>>>>>>> 5c832620657361ca3667d25bc90117f52030772f
            return this.Ok(suggestion);
        }

        [HttpPost("Select")]
        public IActionResult SelectSuggestion([FromBody] SelectQuery selectQuery)
        {
            var isContextUpdated = this._suggestionsService.UpdateContextWithSelectedSuggestion(this.ConversationContext, selectQuery.Id.GetValueOrDefault());
            if (!isContextUpdated)
            {
                return this.BadRequest($"Could not find any suggestion with id: {selectQuery.Id}");
            }

            Log.Debug($"Select suggestion with id {selectQuery.Id}");
            return this.Ok();
        }

        [HttpDelete("Facets")]
        public IActionResult RemoveAllFacets([FromBody] Query query)
        {
            this._questionsService.RejectAllAnswers(this.ConversationContext);
<<<<<<< HEAD
            Log.Debug("Removed all facets");
            return this.NoContent();
=======
            var suggestions = this._suggestionsService.GetSuggestion(this.ConversationContext, query);
            Log.Debug($"Remove all facets");
            return this.Ok(suggestions);
>>>>>>> 5c832620657361ca3667d25bc90117f52030772f
        }

        [HttpDelete("Facets/{id}")]
        public IActionResult RemoveFacet([FromRoute] Guid id, [FromBody] Query query)
        {
            if (!this._questionsService.RejectAnswer(this.ConversationContext, id))
            {
                return this.BadRequest($"Question with id {id} doesn't exist.");
            }

<<<<<<< HEAD
            Log.Debug($"Removed facet with id {id}");
            return this.NoContent();
        }

        private static void LogSuggestion(Suggestion suggestion)
        {
            suggestion.SuggestedDocuments?.ForEach(x => Log.Debug($"Title: {x.Title}, Uri: {x.Uri}, PrintableUri: {x.PrintableUri}, Summary: {x.Summary}"));
            suggestion.Questions?.ForEach(x => Log.Debug($"Id: {x.Id}, Text: {x.Text}"));
            suggestion.ActiveFacets?.ForEach(x => Log.Debug($"Id: {x.Id}, Name: {x.Name}, Value: {x.Value}"));
=======
            var suggestions = this._suggestionsService.GetSuggestion(this.ConversationContext, query);
            Log.Debug($"Remove facet with id {id}");
            return this.Ok(suggestions);
>>>>>>> 5c832620657361ca3667d25bc90117f52030772f
        }
    }
}
