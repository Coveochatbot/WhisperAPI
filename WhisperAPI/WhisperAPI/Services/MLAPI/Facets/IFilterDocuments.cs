using System.Collections.Generic;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public interface IFilterDocuments
    {
        List<string> FilterDocumentsByFacets(FilterDocumentsParameters parameters);
    }
}