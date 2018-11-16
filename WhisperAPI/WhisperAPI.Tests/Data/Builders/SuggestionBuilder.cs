using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SuggestionBuilder
    {
        private List<QuestionToClient> _questions;

        private List<Document> _documents;

        public static SuggestionBuilder Build => new SuggestionBuilder();

        public Suggestion Instance => new Suggestion
        {
            Documents = this._documents,
            Questions = this._questions
        };

        private SuggestionBuilder()
        {
            this._questions = new List<QuestionToClient>();
            this._documents = new List<Document>();
        }

        public SuggestionBuilder WithQuestions(List<QuestionToClient> questions)
        {
            this._questions = questions;
            return this;
        }

        public SuggestionBuilder WithDocuments(List<Document> documents)
        {
            this._documents = documents;
            return this;
        }
    }
}
