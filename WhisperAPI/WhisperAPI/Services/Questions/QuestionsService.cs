using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Services.Questions
{
    public class QuestionsService : IQuestionsService
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool DetectQuestionAsked(ConversationContext context, SearchQuery message)
        {
            if (message.Type == SearchQuery.MessageType.Customer)
            {
                return false;
            }


            bool detectedAskedQuestion = false;
            foreach (var clickedQuestion in context.ClickedQuestions)
            {
                if (clickedQuestion.Text.Contains(message.Query))
                {
                    clickedQuestion.Status = QuestionStatus.AnswerPending;
                    detectedAskedQuestion = true;
                }
            }

            return detectedAskedQuestion;
        }

        public bool DetectAnswer(ConversationContext context, SearchQuery message)
        {
            if (message.Type == SearchQuery.MessageType.Agent)
            {
                return false;
            }

            bool detectedAnswer = false;
            string messageText = message.Query;
            foreach (var pendingQuestion in context.AnswerPendingQuestions)
            {
                if (this.Answered(pendingQuestion, messageText))
                {
                    this.UpdateQuestionWithAnswer(pendingQuestion, messageText);
                    detectedAnswer = true;
                }
            }

            return detectedAnswer;
        }

        public void RejectAllAnswers(ConversationContext context)
        {
            foreach (var contextQuestion in context.Questions)
            {
                this.RejectAnswer(context, contextQuestion.Id);
            }
        }

        public bool RejectAnswer(ConversationContext context, Guid questionId)
        {
            var question = context.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question != null)
            {
                question.Status = QuestionStatus.Rejected;
                return true;
            }

            return false;
        }

        private bool Answered(Question pendingQuestion, string messageText)
        {
            switch (pendingQuestion)
            {
                case FacetQuestion facetQuestion:
                    return this.Answered(facetQuestion, messageText);
            }

            throw new NotSupportedException("Question type not supported");
        }

        private bool Answered(FacetQuestion pendingQuestion, string messageText)
        {
            return pendingQuestion.FacetValues.Any(facet => messageText.Contains(facet));
        }

        private void UpdateQuestionWithAnswer(Question question, string messageText)
        {
            switch (question)
            {
                case FacetQuestion facetQuestion:
                    this.UpdateQuestionWithAnswer(facetQuestion, messageText);
                    return;
            }

            throw new NotSupportedException("Question type not supported.");
        }

        private void UpdateQuestionWithAnswer(FacetQuestion question, string messageText)
        {
            question.Status = QuestionStatus.Answered;
            question.Answer = question.FacetValues.FirstOrDefault(facet => messageText.Contains(facet));
        }
    }
}
