using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Models;
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

        [HttpPost]
        [Route("")]
        public IActionResult GetSuggestions([FromBody] SearchQuerry searchQuerry)
        {
            if (this.ModelState.IsValid)
            {
                return this.Ok(this._suggestionsService.GetSuggestion(searchQuerry.Querry).ToList());
            }
            else
            {
                return this.BadRequest();
            }
        }
    }
}
