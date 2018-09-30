using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Questions
{
    public class QuestionsService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void DetectAnswer(ConversationContext context, SearchQuery message)
        {
            if (message.Type == SearchQuery.MessageType.Agent)
                return;

            string messageText = message.Query;
            foreach (var pendingQuestion in context.SelectedQuestions)
            {
                if (Answered(pendingQuestion, messageText))
                {
                    UpdateQuestionWithAnswer(pendingQuestion, messageText);
                }
            }
        }

        private void UpdateQuestionWithAnswer(Question question, string messageText)
        {
            switch(question)
            {
                case FacetQuestion facetQuestion:
                    UpdateQuestionWithAnswer(facetQuestion, messageText);
                    return;
            }

            throw new NotSupportedException("Question type not supported.");
        }

        private void UpdateQuestionWithAnswer(FacetQuestion question, string messageText)
        {
            question.Status = QuestionStatus.Answered;
            question.Answer = question.FacetValues.FirstOrDefault(facet => messageText.Contains(facet));
        }

        public bool Answered(Question pendingQuestion, string messageText)
        {
            switch(pendingQuestion)
            {
                case FacetQuestion facetQuestion:
                    return Answered(facetQuestion, messageText);
            }

            throw new NotSupportedException("Question type not supported");
        }

        public bool Answered(FacetQuestion pendingQuestion, string messageText)
        {
            return pendingQuestion.FacetValues.Any(facet => messageText.Contains(facet));
        }
    }
}
