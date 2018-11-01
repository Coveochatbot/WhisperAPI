using System.Collections.Generic;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Models
{
    public class Suggestion
    {
        public List<QuestionToClient> Questions { get; set; }

        public List<Document> Documents { get; set; }

        public List<Facet> ActiveFacets { get; set; }
    }
}