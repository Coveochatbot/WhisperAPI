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
        public IActionResult GetSuggestions([FromBody] SearchQuery searchQuery)
        {
            if (!this.ModelState.IsValid || searchQuery?.Query == null)
            {
                return this.BadRequest();
            }

            this._suggestionsService.UpdateContextWithNewQuery(this.ConversationContext, searchQuery);
            var suggestions = this._suggestionsService.GetSuggestions(this.ConversationContext).ToList();
            this._suggestionsService.UpdateContextWithNewSuggestions(this.ConversationContext, suggestions);

            return this.Ok(suggestions);
        }
    }
}
