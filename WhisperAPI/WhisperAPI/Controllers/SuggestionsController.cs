using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Services;

namespace WhisperAPI.Controllers
{
    [Route("/Whisper/[Controller]")]
    public class SuggestionsController : Controller
    {
        private readonly ISuggestionsService _suggestionsService;

        public SuggestionsController(ISuggestionsService suggestionsService)
        {
            this._suggestionsService = suggestionsService;
        }

        [HttpGet]
        public IActionResult GetSuggestions()
        {
            return this.Ok("Hello World");
        }
    }
}
