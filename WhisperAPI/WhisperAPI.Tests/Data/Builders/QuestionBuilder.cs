using System;
using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class QuestionBuilder
    {
        private string _text;

        private Guid _id;

        public static QuestionBuilder Build => new QuestionBuilder();

        public Question Instance => new Question
        {
            Text = this._text,
            Id = this._id
        };

        private QuestionBuilder()
        {
            this._text = "Text";
            this._id = Guid.NewGuid();
        }

        public QuestionBuilder WithText(string text)
        {
            this._text = text;
            return this;
        }

        public QuestionBuilder WithId(Guid id)
        {
            this._id = id;
            return this;
        }
    }
}
