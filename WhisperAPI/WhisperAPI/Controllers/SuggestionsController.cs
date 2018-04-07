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
        private readonly ISuggestionsService _suggestionsService;

        public SuggestionsController(ISuggestionsService suggestionsService, Contexts contexts)
            : base(contexts)
        {
            this._suggestionsService = suggestionsService;
        }

        [HttpPost]
        public IActionResult GetSuggestions([FromBody] SearchQuerry searchQuerry)
        {
            if (!this.ModelState.IsValid || searchQuerry?.Querry == null)
            {
                return this.BadRequest();
            }

            var conversationContext = this._contexts[searchQuerry.ChatKey];
            conversationContext.MessagesSuggestions.Add(new MessageSuggestion(searchQuerry.Querry));

            var allConversation = string.Join(" ", conversationContext.MessagesSuggestions.Select(m => m.Message));

            return this.Ok(this._suggestionsService.GetSuggestions(allConversation).ToList());
        }
    }
}
