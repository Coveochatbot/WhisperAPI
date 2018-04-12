using System;
using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SearchQueryBuilder
    {
        private Guid? _chatKey = Guid.NewGuid();

        private string _query = "Test";

        private SearchQuerry.MessageType _type = 0;

        public SearchQueryBuilder WithChatKey(Guid? chatKey)
        {
            this._chatKey = chatKey;
            return this;
        }

        public SearchQueryBuilder WithQuery(string query)
        {
            this._query = query;
            return this;
        }

        public SearchQueryBuilder WithMessageType(SearchQuerry.MessageType type)
        {
            this._type = type;
            return this;
        }

        public SearchQuerry Build() => new SearchQuerry
        {
            ChatKey = this._chatKey,
            Querry = this._query,
            Type = this._type
        };
    }
}
