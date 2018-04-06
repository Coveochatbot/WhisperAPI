using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class Contexts : DbContext
    {
        private bool _removeOldLock = false;

        public Contexts(DbContextOptions<Contexts> options, TimeSpan contextLifeSpan)
            : base(options)
        {
            this.ContextLifeSpan = contextLifeSpan;
        }

        public TimeSpan ContextLifeSpan { get; set; }

        private DbSet<ConversationContext> ConversationContexts { get; set; }

        // Get the Conversation associated to the chatkey,
        // create a new one if doesn't already exist
        public ConversationContext this[Guid chatkey]
        {
            get
            {
                ConversationContext conversationContext = this.ConversationContexts
                    .FirstOrDefault(x => x.ChatKey == chatkey);

                if (conversationContext == null)
                {
                    conversationContext = new ConversationContext(chatkey, DateTime.Now);
                    this.ConversationContexts.Add(conversationContext);
                    this.SaveChangesAsync();
                }

                return conversationContext;
            }
        }

        public List<ConversationContext> RemoveOldContext()
        {
            List<ConversationContext> removedContexts = this.ConversationContexts.Where(x => ((DateTime.Now - x.StartDate) > this.ContextLifeSpan)).ToList();
            if (this._removeOldLock)
            {
                // if lock is locked, still return the list of contexts that will be removed by the other thread
                return removedContexts;
            }

            this._removeOldLock = true;

            foreach (var context in removedContexts)
            {
                this.ConversationContexts.Remove(context);
            }

            this.SaveChangesAsync();
            this._removeOldLock = false;

            return removedContexts;
        }
    }
}
