using System.Collections.Generic;

namespace WhisperAPI.Models.MLAPI
{
    public class FilterDocumentsParameters
    {
        public List<string> Documents { get; set; }

        public List<Facet> MustHaveFacets { get; set; }

        public List<Facet> MustNotHaveFacets { get; set; }
    }
}
