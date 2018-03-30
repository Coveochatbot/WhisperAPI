using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class ConversationContext : DbContext
    {
        public ConversationContext(DbContextOptions<ConversationContext> options)
            : base(options)
        {
        }

        private DbSet<Conversation> ConversationMessages { get; set; }

        // Get the Conversation associated to the chatkey,
        // create a new one if doesn't already exist
        public Conversation this[string chatkey]
        {
            get
            {
                Conversation conversation = this.ConversationMessages
                    .Where(x => x.ChatKey == chatkey).FirstOrDefault();

                if (conversation == null)
                {
                    conversation = new Conversation(chatkey, DateTime.Now);
                    this.Add(conversation);
                    this.SaveChangesAsync();
                }

                return conversation;
            }

            set
            {
                Conversation conversation = this[chatkey];
                conversation = value;

                this.SaveChangesAsync();
            }
        }
    }
}
