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

        private void UpdateQuestionWithAnswer(object question, string messageText)
        {
            throw new NotImplementedException();
        }

        private bool Answered(Question pendingQuestion, string messageText)
        {
            throw new NotImplementedException();
        }
    }
}
