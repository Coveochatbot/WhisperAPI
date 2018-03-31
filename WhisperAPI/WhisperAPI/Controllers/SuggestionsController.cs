using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WhisperAPI.Models;
using WhisperAPI.Services;
using static WhisperAPI.Models.SearchQuerry;

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
        public IActionResult GetSuggestions([FromBody] SearchQuerry searchQuerry)
        {
            if (searchQuerry?.ChatKey == null || searchQuerry?.Querry == null || searchQuerry?.Type == MessageType.Error)
            {
                return this.BadRequest();
            }

            return this.Ok(this._suggestionsService.GetSuggestions(searchQuerry.Querry).ToList());
        }
    }
}
