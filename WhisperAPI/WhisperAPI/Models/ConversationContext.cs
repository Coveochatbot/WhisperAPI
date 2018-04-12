using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WhisperAPI.Models
{
    public class ConversationContext
    {
        public ConversationContext(Guid? chatkey, DateTime datetime)
            : this()
        {
            this.ChatKey = chatkey;
            this.StartDate = datetime;
        }

        public ConversationContext()
        {
            this.SearchQuerries = new List<SearchQuerry>();
            this.SuggestedDocuments = new HashSet<SuggestedDocument>();
        }

        [Key]
        public Guid? ChatKey { get; set; }

        public DateTime StartDate { get; set; }

        public List<SearchQuerry> SearchQuerries { get; set; }

        public HashSet<SuggestedDocument> SuggestedDocuments { get; set; }
    }
}
