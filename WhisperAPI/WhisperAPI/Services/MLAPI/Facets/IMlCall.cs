using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public interface IMlCall
    {
        FacetAnalysis GetFacetAnalysis(IEnumerable<SuggestedDocument> suggestedDocuments);
    }
}