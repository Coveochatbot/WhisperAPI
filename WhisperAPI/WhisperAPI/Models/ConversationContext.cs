using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WhisperAPI.Models
{
    public class ConversationContext
    {
        public ConversationContext(Guid chatkey, DateTime datetime)
        {
            this.ChatKey = chatkey;
            this.StartDate = datetime;
            this.MessagesSuggestions = new List<MessageSuggestion>();
        }

        private ConversationContext()
        {
            this.MessagesSuggestions = new List<MessageSuggestion>();
        }

        [Key]
        public Guid ChatKey { get; set; }

        public DateTime StartDate { get; set; }

        public ICollection<MessageSuggestion> MessagesSuggestions { get; set; }

        public string GetAllMessages()
        {
            var allMessages = string.Empty;
            if (this.MessagesSuggestions != null)
            {
                allMessages = string.Join(" ", this.MessagesSuggestions.Select(m => m.Message));
            }

            return allMessages;
        }
    }
}
