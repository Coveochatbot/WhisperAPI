using System;
using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Suggestions
{
    public interface ISuggestionsService
    {
        Suggestion GetNewSuggestion(ConversationContext conversationContext, SuggestionQuery query);

        Suggestion GetLastSuggestion(ConversationContext conversationContext, SuggestionQuery query);

        IEnumerable<Document> GetDocuments(ConversationContext conversationContext);

        IEnumerable<Question> GetQuestionsFromDocument(ConversationContext conversationContext, IEnumerable<Document> documents);

        List<Document> FilterDocumentsByFacet(ConversationContext conversationContext);

        void UpdateContextWithNewQuery(ConversationContext conversationContext, SearchQuery searchQuery);

        void UpdateContextWithNewSuggestions(ConversationContext conversationContext, List<Document> documents);

        void UpdateContextWithNewQuestions(ConversationContext conversationContext, List<Question> questions);

        bool UpdateContextWithSelectedSuggestion(ConversationContext conversationContext, Guid selectQueryId);
    }
}
