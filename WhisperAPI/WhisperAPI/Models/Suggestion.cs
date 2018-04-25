using System.Collections.Generic;

namespace WhisperAPI.Models
{
    public class Suggestion
    {
        public List<Question> Questions { get; set; }

        public List<SuggestedDocument> SuggestedDocuments { get; set; }
    }
}