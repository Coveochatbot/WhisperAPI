using System;
using System.Collections.Generic;
using System.Linq;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class InMemoryContexts : IContexts
    {
        private object _removeOldLock = new object();

        public InMemoryContexts(TimeSpan contextLifeSpan)
        {
            this.ContextLifeSpan = contextLifeSpan;
        }

        public TimeSpan ContextLifeSpan { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private Dictionary<Guid, ConversationContext> ConversationContexts { get; set; }

        public ConversationContext this[Guid chatkey]
        {
            get
            {
                if (!this.ConversationContexts.TryGetValue(chatkey, out ConversationContext conversationContext))
                {
                    this.RemoveOldContext();

                    conversationContext = new ConversationContext(chatkey, DateTime.Now);
                    this.ConversationContexts.Add(chatkey, conversationContext);
                }

                return conversationContext;
            }
        }

        public List<ConversationContext> RemoveOldContext()
        {
            lock (this._removeOldLock)
            {
                List<ConversationContext> removedContexts = this.ConversationContexts
                    .Where(x => ((DateTime.Now - x.Value.StartDate) > this.ContextLifeSpan))
                    .Select(x => x.Value).ToList();

                foreach (var context in removedContexts)
                {
                    this.ConversationContexts.Remove(context.ChatKey);
                }

                return removedContexts;
            }
        }

    }
}
