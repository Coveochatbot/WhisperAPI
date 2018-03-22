using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class IndexSearch : IIndexSearch
    {
        private const string URL = "https://cloudplatform.coveo.com/rest/search/v2";
        private readonly IHttpClientWrapper _httpClientWrapper;

        public IndexSearch(IHttpClientWrapper httpClientWrapper)
        {
            this._httpClientWrapper = httpClientWrapper;
        }

        public ISearchResult Search(string querry)
        {
            return JsonConvert.DeserializeObject<SearchResult>(this._httpClientWrapper.GetStringFromPost(URL, this.CreateStringContent(querry)));
        }

        private StringContent CreateStringContent(string querry)
        {
            return new StringContent($"{{\"q\" : \"{querry}\"}}");
        }
    }
}
