using System.Collections.Generic;
using WhisperAPI.Models;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public interface IDocumentFacets
    {
        List<FacetQuestion> GetQuestions(IEnumerable<string> documentsUri);
    }
}