using System;
using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.MLAPI;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Suggestions
{
    public interface ISuggestionsService
    {
        IEnumerable<SuggestedDocument> GetSuggestedDocuments(ConversationContext conversationContext);

        IEnumerable<Question> GetQuestionsFromDocument(ConversationContext conversationContext, IEnumerable<SuggestedDocument> suggestedDocuments);

        List<string> FilterDocumentsByFacet(FilterDocumentsParameters parameters);

        void UpdateContextWithNewQuery(ConversationContext conversationContext, SearchQuery searchQuery);

        void UpdateContextWithNewSuggestions(ConversationContext conversationContext, List<SuggestedDocument> suggestedDocuments);

        void UpdateContextWithNewQuestions(ConversationContext conversationContext, List<Question> questions);

        bool UpdateContextWithSelectedSuggestion(ConversationContext conversationContext, Guid selectQueryId);
    }
}
