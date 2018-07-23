using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SuggestionBuilder
    {
        private List<Question> _questions;

        private List<SuggestedDocument> _suggestedDocuments;

        public static SuggestionBuilder Build => new SuggestionBuilder();

        public Suggestion Instance => new Suggestion
        {
            SuggestedDocuments = this._suggestedDocuments,
            Questions = this._questions
        };

        private SuggestionBuilder()
        {
            this._questions = new List<Question>();
            this._suggestedDocuments = new List<SuggestedDocument>();
        }

        public SuggestionBuilder WithQuestions(List<Question> questions)
        {
            this._questions = questions;
            return this;
        }

        public SuggestionBuilder WithSuggestedDocuments(List<SuggestedDocument> suggestedDocuments)
        {
            this._suggestedDocuments = suggestedDocuments;
            return this;
        }
    }
}
