using System;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Questions
{
    public interface IQuestionsService
    {
        bool DetectQuestionAsked(ConversationContext context, SearchQuery message);

        bool DetectAnswer(ConversationContext context, SearchQuery message);

        bool RejectAnswer(ConversationContext context, Guid questionId);

        void RejectAllAnswers(ConversationContext context);
    }
}