using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public interface ISuggestionsService
    {
        IEnumerable<SuggestedDocument> GetSuggestions(string querry);
    }
}
