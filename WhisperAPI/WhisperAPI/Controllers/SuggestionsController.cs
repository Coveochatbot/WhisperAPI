﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Models;
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
            this._questionsService.DetectQuestionAsked(this.ConversationContext, searchQuery);
            var suggestion = this._suggestionsService.GetSuggestion(this.ConversationContext);
            return this.Ok(suggestion);
        }

        [HttpGet]
        public IActionResult GetSuggestions(Query query)
        {
            Log.Debug($"Query: {query}");
            var mustHaveFacets = this.ConversationContext.AnsweredQuestions.OfType<FacetQuestion>().Select(a => new Facet
            {
                Id = a.Id,
                Name = a.FacetName,
                Value = a.Answer
            }).ToList();

            var suggestion = new Suggestion()
            {
                SuggestedDocuments = this.ConversationContext.LastSuggestedDocuments,
                Questions = this.ConversationContext.LastSuggestedQuestions.Select(QuestionToClient.FromQuestion).ToList(),
                ActiveFacets = mustHaveFacets
            };

            suggestion.SuggestedDocuments.ForEach(x => Log.Debug($"Title: {x.Title}, Uri: {x.Uri}, PrintableUri: {x.PrintableUri}, Summary: {x.Summary}"));
            suggestion.Questions.ForEach(x => Log.Debug($"Id: {x.Id}, Text: {x.Text}"));
            suggestion.ActiveFacets.ForEach(x => Log.Debug($"Id: {x.Id}, Name: {x.Name}, Value: {x.Value}"));
            return this.Ok(suggestion);
        }

        [HttpPost("Select")]
        public IActionResult SelectSuggestion([FromBody] SelectQuery selectQuery)
        {
            bool isContextUpdated = this._suggestionsService.UpdateContextWithSelectedSuggestion(this.ConversationContext, selectQuery.Id.GetValueOrDefault());
            if (!isContextUpdated)
            {
                return this.BadRequest();
            }

            Log.Debug($"Select suggestion with id {selectQuery.Id}");

            return this.Ok();
        }

        [HttpDelete("Facets")]
        public IActionResult RemoveAllFacets([FromBody] Query query)
        {
            // TODO Validate if it is reject
            this._questionsService.RejectAllAnswers(this.ConversationContext);
            var suggestions = this._suggestionsService.GetSuggestion(this.ConversationContext);
            Log.Debug($"Remove all facets");
            return this.Ok(suggestions);
        }

        [HttpDelete("Facets/{id}")]
        public IActionResult RemoveFacet([FromRoute][Required] Guid? id, [FromBody] Query query)
        {
            if (!this._questionsService.RejectAnswer(this.ConversationContext, id.GetValueOrDefault()))
            {
                return this.BadRequest($"Question with id {id} doesn't exist.");
            }

            var suggestions = this._suggestionsService.GetSuggestion(this.ConversationContext);
            Log.Debug($"Remove facet with id {id}");
            return this.Ok(suggestions);
        }
    }
}
