using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class Contexts : DbContext
    {
        public Contexts(DbContextOptions<Contexts> options)
            : base(options)
        {
        }

        private DbSet<ConversationContext> ConversationContext { get; set; }

        // Get the Conversation associated to the chatkey,
        // create a new one if doesn't already exist
        public ConversationContext this[string chatkey]
        {
            get
            {
                ConversationContext conversationContext = this.ConversationContext
                    .FirstOrDefault(x => x.ChatKey == chatkey);

                if (conversationContext == null)
                {
                    conversationContext = new ConversationContext(chatkey, DateTime.Now);
                    this.Add(conversationContext);
                    this.SaveChangesAsync();
                }

                return conversationContext;
            }
        }
    }
}
