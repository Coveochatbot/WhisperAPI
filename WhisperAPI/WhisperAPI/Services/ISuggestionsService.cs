using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public interface ISuggestionsService
    {
        IEnumerable<SuggestedDocument> GetSuggestions(ConversationContext conversationContext);

        void UpdateContextWithNewQuery(ConversationContext conversationContext, SearchQuerry searchQuerry);

        void UpdateContextWithNewSuggestions(ConversationContext conversationContext, List<SuggestedDocument> suggestedDocuments);
    }
}
