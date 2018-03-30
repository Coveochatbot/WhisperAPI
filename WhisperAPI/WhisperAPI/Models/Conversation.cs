using System;
using System.Collections.Generic;

namespace WhisperAPI.Models
{
    public class Conversation
    {
        public Conversation(string chatkey, DateTime dateTime)
        {
            this.ChatKey = chatkey;
        }

        public string ChatKey { get; }

        public DateTime StartDate { get; }

        public IEnumerable<ConversationMessage> Messages { get; set; }
    }
}
