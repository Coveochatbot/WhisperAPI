using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Controllers
{
    [Route("/Whisper/[Controller]")]
    public class SuggestionsController : ContextController
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISuggestionsService _suggestionsService;

        public SuggestionsController(ISuggestionsService suggestionsService, IContexts contexts)
            : base(contexts)
        {
            this._suggestionsService = suggestionsService;
        }

        [HttpPost]
        public IActionResult GetSuggestions([FromBody] SearchQuery searchQuery)
        {
            if (!this.ModelState.IsValid || searchQuery?.Query == null)
            {
                return this.BadRequest();
            }

            this._suggestionsService.UpdateContextWithNewQuery(this.ConversationContext, searchQuery);
            var suggestions = this._suggestionsService.GetSuggestions(this.ConversationContext).ToList();
            this._suggestionsService.UpdateContextWithNewSuggestions(this.ConversationContext, suggestions);

            suggestions.ForEach(x =>
                Log.Debug($"Title: {x.Title}, Uri: {x.Uri}, PrintableUri: {x.PrintableUri}, Summary: {x.Summary}"));

            return this.Ok(suggestions);
        }

        [HttpGet]
        public IActionResult GetSuggestions(Guid chatkey)
        {
            Log.Debug($"Chatkey: {chatkey}");
            return this.Ok(this.ConversationContext.LastSuggestedDocuments);
        }
    }
}
