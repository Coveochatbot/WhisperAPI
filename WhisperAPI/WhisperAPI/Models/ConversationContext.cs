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

        public ConversationContext(string chatkey, DateTime datetime)
        {
            this.ChatKey = chatkey;
            this.StartDate = datetime;
        }

        [Key]
        public string ChatKey { get; set; }

        public DateTime StartDate { get; set; }
    }
}
