using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models;

namespace WhisperAPI.Services
{
    public class IndexSearch : IIndexSearch
    {
        private const string URL = "https://cloudplatform.coveo.com/rest/search/v2";
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public IndexSearch(string apiKey, HttpClient client)
        {
            this._apiKey = apiKey;
            this._httpClient = client;
        }

        public ISearchResult Search(string querry)
        {
            return JsonConvert.DeserializeObject<SearchResult>(this.GetStringFromPost(URL, this.CreateStringContent(querry)));
        }

        private string GetStringFromPost(string url, StringContent content)
        {
            this.InitHttpClient();
            HttpResponseMessage response = this._httpClient.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                // throw error
                return string.Empty;
            }
        }

        private StringContent CreateStringContent(string querry)
        {
            // sanitize
            querry.Replace("\"", string.Empty);

            return new StringContent($"{{\"lq\": \"{querry}\"}}", Encoding.UTF8, "application/json");
        }

        private void InitHttpClient()
        {
            // Add an Accept header for JSON format.
            this._httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // Add an Authorization header with the ApiKey
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this._apiKey}");
        }
    }
}
