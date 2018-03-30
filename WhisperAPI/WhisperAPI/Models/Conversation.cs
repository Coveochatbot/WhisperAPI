using System;
using System.Collections.Generic;

namespace WhisperAPI.Models
{
    public class Conversation
    {
        public string ChatKey { get; set; }

        public DateTime StartDate { get; set; }

        public IEnumerable<ConversationMessage> Messages { get; set; }
    }
}
