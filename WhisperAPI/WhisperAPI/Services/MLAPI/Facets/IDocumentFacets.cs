using System.Collections.Generic;
using WhisperAPI.Models;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public interface IDocumentFacets
    {
        FacetAnalysis GetFacetAnalysis(IEnumerable<SuggestedDocument> suggestedDocuments);
    }
}