using System;
using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.MLAPI;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Suggestions
{
    public interface ISuggestionsService
    {
        Suggestion GetNewSuggestion(ConversationContext conversationContext);

        Suggestion GetLastSuggestion(ConversationContext conversationContext);

        IEnumerable<Document> GetDocuments(ConversationContext conversationContext);

        IEnumerable<Question> GetQuestionsFromDocument(ConversationContext conversationContext, IEnumerable<Document> documents);

        List<Document> FilterDocumentsByFacet(ConversationContext conversationContext, List<Facet> mustHaveFacets);

        void UpdateContextWithNewQuery(ConversationContext conversationContext, SearchQuery searchQuery);

        void UpdateContextWithNewSuggestions(ConversationContext conversationContext, List<Document> documents);

        void UpdateContextWithNewQuestions(ConversationContext conversationContext, List<Question> questions);

        bool UpdateContextWithSelectedSuggestion(ConversationContext conversationContext, Guid selectQueryId);
    }
}
