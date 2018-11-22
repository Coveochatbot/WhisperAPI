using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Models;
using WhisperAPI.Models.MegaGenial;
using WhisperAPI.Models.MLAPI;
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

        public Module CurrentDetectedModule { get; set; }

        public SuggestionsController(ISuggestionsService suggestionsService, IQuestionsService questionsService, IContexts contexts)
            : base(contexts)
        {
            this._suggestionsService = suggestionsService;
            this._questionsService = questionsService;
        }

        [HttpPost]
        public IActionResult GetSuggestions([FromBody] SearchQuery searchQuery)
        {
            var currentDetectedModulesAndMatchScore = new ModuleDetector().DetectModuleList(searchQuery.Query);
            var currentDetectedModules = from module in currentDetectedModulesAndMatchScore select module.Item1;
            if (currentDetectedModules.Any())
            {
                if (!currentDetectedModules.Contains(this.CurrentDetectedModule))
                {
                    this.CurrentDetectedModule = currentDetectedModules.First();
                    var previousConversationContext = this.ConversationContext;
                    this.ConversationContext = new ConversationContext(
                        previousConversationContext.ChatKey, previousConversationContext.StartDate);
                }
            }

            this._suggestionsService.UpdateContextWithNewQuery(this.ConversationContext, searchQuery);

            if (searchQuery.Type == SearchQuery.MessageType.Agent)
            {
                searchQuery.Relevant = false;
            }

            var suggestion = this._suggestionsService.GetNewSuggestion(this.ConversationContext, searchQuery);

            LogSuggestion(suggestion);
            return this.Ok(suggestion);
        }

        [HttpGet]
        public IActionResult GetSuggestions(SuggestionQuery suggestionQuery)
        {
            Log.Debug($"SuggestionQuery: {suggestionQuery}");

            var suggestion = this._suggestionsService.GetLastSuggestion(this.ConversationContext, suggestionQuery);

            LogSuggestion(suggestion);

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

            Log.Debug("Removed all facets");
            return this.NoContent();
        }

        [HttpDelete("Facets/{id}")]
        public IActionResult RemoveFacet([FromRoute] Guid id, [FromBody] Query query)
        {
            if (!this._questionsService.RejectAnswer(this.ConversationContext, id))
            {
                return this.BadRequest($"Question with id {id} doesn't exist.");
            }

            Log.Debug($"Removed facet with id {id}");
            return this.NoContent();
        }

        [HttpPut("Filter")]
        public IActionResult AddFilter([FromBody] FilterQuery query)
        {
            this.ConversationContext.FilterDocumentsParameters.MustHaveFacets.Add(query.Facet);
            return this.NoContent();
        }

        [HttpDelete("Filter")]
        public IActionResult RemoveFilter([FromBody] FilterQuery query)
        {
            this.ConversationContext.FilterDocumentsParameters.MustHaveFacets.Remove(query.Facet);
            return this.NoContent();
        }

        private static void LogSuggestion(Suggestion suggestion)
        {
            suggestion.Documents?.ForEach(x => Log.Debug($"Title: {x.Title}, Uri: {x.Uri}, PrintableUri: {x.PrintableUri}, Summary: {x.Summary}"));
            suggestion.Questions?.ForEach(x => Log.Debug($"Id: {x.Id}, Text: {x.Text}"));
            suggestion.ActiveFacets?.ForEach(x => Log.Debug($"Id: {x.Id}, Name: {x.Name}, Value: {x.Value}"));
        }
    }
}
