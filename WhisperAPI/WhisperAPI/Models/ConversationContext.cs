using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhisperAPI.Models
{
    public class ConversationContext
    {
        public ConversationContext()
        {
        }

        public ConversationContext(string chatkey)
        {
            this.ChatKey = chatkey;
        }

        [Key]
        public string ChatKey { get; }

        public DateTime StartDate { get; set; }
    }
}
