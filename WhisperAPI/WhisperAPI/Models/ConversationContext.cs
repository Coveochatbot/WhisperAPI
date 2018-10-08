using System;
using System.Collections.Generic;
using System.Linq;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Models
{
    public class ConversationContext
    {
        public ConversationContext(Guid chatkey, DateTime datetime)
            : this()
        {
            this.ChatKey = chatkey;
            this.StartDate = datetime;
        }

        public ConversationContext()
        {
            this.SearchQueries = new List<SearchQuery>();
            this.SuggestedDocuments = new HashSet<SuggestedDocument>();
            this.LastSuggestedDocuments = new List<SuggestedDocument>();
            this.LastSuggestedQuestions = new List<Question>();
            this.Questions = new HashSet<Question>();
            this.SelectedSuggestedDocuments = new HashSet<SuggestedDocument>();
        }

        public Guid ChatKey { get; set; }

        public DateTime StartDate { get; set; }

        public List<SearchQuery> SearchQueries { get; set; }

        public HashSet<SuggestedDocument> SuggestedDocuments { get; set; }

        public HashSet<SuggestedDocument> SelectedSuggestedDocuments { get; set; }

        public HashSet<Question> Questions { get; set; }

        public IReadOnlyList<Question> ClickedQuestions => this.Questions.Where(q => q.Status == QuestionStatus.Clicked).ToList();

        public IReadOnlyList<Question> AnswerPendingQuestions => this.Questions.Where(q => q.Status == QuestionStatus.AnswerPending).ToList();

        public IReadOnlyList<Question> AnsweredQuestions => this.Questions.Where(q => q.Status == QuestionStatus.Answered).ToList();

        public List<SuggestedDocument> LastSuggestedDocuments { get; set; }

        public List<Question> LastSuggestedQuestions { get; set; }
    }
}
