using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public interface ISuggestionsService
    {
        IEnumerable<SuggestedDocument> GetSuggestion(string querry);
    }
}
