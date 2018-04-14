using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public interface IIndexSearch
    {
        ISearchResult Search(string query);
    }
}
