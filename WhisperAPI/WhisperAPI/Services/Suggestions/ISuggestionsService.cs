using System;
using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Suggestions
{
    public interface ISuggestionsService
    {
        Suggestion GetNewSuggestion(ConversationContext conversationContext);

        Suggestion GetLastSuggestion(ConversationContext conversationContext);

        IEnumerable<Document> GetDocuments(ConversationContext conversationContext);

        void UpdateContextWithNewQuery(ConversationContext conversationContext, SearchQuery searchQuery);

        bool UpdateContextWithSelectedSuggestion(ConversationContext conversationContext, Guid selectQueryId);
    }
}
