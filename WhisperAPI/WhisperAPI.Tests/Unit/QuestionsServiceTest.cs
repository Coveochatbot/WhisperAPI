using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WhisperAPI.Models;
using WhisperAPI.Models.Queries;
using WhisperAPI.Models.Search;
using WhisperAPI.Services.Questions;
using WhisperAPI.Tests.Data.Builders;
using static WhisperAPI.Models.Queries.SearchQuery;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class QuestionsServiceTest
    {
        private IQuestionsService _questionsService;

        private ConversationContext _conversationContext;

        [SetUp]
        public void SetUp()
        {
            this._questionsService = new QuestionsService();
            this._conversationContext = new ConversationContext(new Guid("a21d07d5-fd5a-42ab-ac2c-2ef6101e58d9"), DateTime.Now);
        }

        [Test]
        [TestCase]
        public void When_receive_agent_message_then_no_answer_detected()
        {
            var agentQuery = SearchQueryBuilder.Build.WithMessageType(SearchQuery.MessageType.Agent).Instance;
            Assert.IsFalse(this._questionsService.DetectAnswer(null, agentQuery));
        }

        [TestCase]
        public void When_receive_custommer_message_with_answer_to_pending_question_then_answer_detected()
        {
            var pendingQuestion = FacetQuestionBuilder.Build.WithStatus(Models.QuestionStatus.AnswerPending).WithFacetName("Dummy").WithFacetValues("A", "B").Instance;
            this._conversationContext.Questions.Add(pendingQuestion);
            var questionAnswer = SearchQueryBuilder.Build.WithMessageType(MessageType.Customer).WithQuery("It is A").Instance;

            Assert.IsTrue(this._questionsService.DetectAnswer(this._conversationContext, questionAnswer));
            Assert.AreEqual("A", ((FacetQuestion)this._conversationContext.AnsweredQuestions.Single()).Answer);
        }

        [TestCase]
        public void When_receive_question_that_was_clicked_in_message_then_detect_question_and_set_to_pending_question()
        {
            var clickedQuestion = FacetQuestionBuilder.Build.WithStatus(Models.QuestionStatus.Clicked).WithFacetName("Dummy").WithFacetValues("A", "B").Instance;
            this._conversationContext.Questions.Add(clickedQuestion);
            var questionAsked = SearchQueryBuilder.Build.WithMessageType(MessageType.Agent).WithQuery(clickedQuestion.Text).Instance;

            Assert.IsTrue(this._questionsService.DetectQuestionAsked(this._conversationContext, questionAsked));
            Assert.IsTrue(clickedQuestion.Status == QuestionStatus.AnswerPending);
        }
    }
}
