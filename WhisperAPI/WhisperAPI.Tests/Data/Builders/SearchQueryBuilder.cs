using System;
using WhisperAPI.Models.Queries;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SearchQueryBuilder
    {
        private Guid? _chatKey;

        private string _query;

        private SearchQuery.MessageType _type;

        public static SearchQueryBuilder Build => new SearchQueryBuilder();

        public SearchQuery Instance => new SearchQuery
        {
            ChatKey = this._chatKey,
            Query = this._query,
            Type = this._type
        };

        private SearchQueryBuilder()
        {
            this._chatKey = Guid.NewGuid();
            this._query = "Test";
            this._type = SearchQuery.MessageType.Customer;
        }

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

        public SearchQueryBuilder WithMessageType(SearchQuery.MessageType type)
        {
            this._type = type;
            return this;
        }
    }
}
