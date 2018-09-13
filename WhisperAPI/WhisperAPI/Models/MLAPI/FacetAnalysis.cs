using System.Collections.Generic;

namespace WhisperAPI.Models.MLAPI
{
    public class FacetAnalysis
    {
        public List<SuggestedDocument> SuggestedDocuments { get; set; }

        public List<Question> Questions { get; set; }
    }
}
