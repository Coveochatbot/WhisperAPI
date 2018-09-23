using System;
using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SelectQueryBuilder
    {
        private Guid? _chatKey;

        private Guid? _id;

        public static SelectQueryBuilder Build => new SelectQueryBuilder();

        public SelectQuery Instance => new SelectQuery
        {
            Id = this._id,
            ChatKey = this._chatKey
        };

        public SelectQueryBuilder()
        {
            this._chatKey = Guid.NewGuid();
            this._id = Guid.NewGuid();
        }

        public SelectQueryBuilder WithId(Guid? id)
        {
            this._id = id;
            return this;
        }

        public SelectQueryBuilder WithChatKey(Guid? chatKey)
        {
            this._chatKey = chatKey;
            return this;
        }
    }
}
