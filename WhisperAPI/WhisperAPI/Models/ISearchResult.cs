using System.Collections.Generic;

namespace WhisperAPI.Models
{
    public interface ISearchResult
    {
        int NbrElements { get; set; }

        IEnumerable<ISearchResultElement> Elements { get; set; }
    }
}
