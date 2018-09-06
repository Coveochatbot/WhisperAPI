using System.Collections.Generic;

namespace ConsoleApp1.Models.Search
{
    public interface ISearchResult
    {
        int NbrElements { get; set; }

        IEnumerable<ISearchResultElement> Elements { get; set; }
    }
}
