using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ConsoleApp1.Models.Search;
using Newtonsoft.Json;

namespace ConsoleApp1.Services
{
    public class IndexSearchSortByField
    {
        private const string URL = "https://cloudplatform.coveo.com/rest/search/v2";
        private readonly string _apiKey = "";
        private readonly HttpClient _httpClient = new HttpClient();

        public IndexSearchSortByField()
        {
            this.InitHttpClient();
        }

        public ISearchResult Search(string fieldname, string value, int rowId)
        {
            var json = this.GetStringFromPost(URL, this.CreateStringContent(fieldname, value, rowId), fieldname, value);
            return JsonConvert.DeserializeObject<SearchResult>(json);
        }

        private string GetStringFromPost(string url, StringContent content, string fieldname, string value)
        {
            HttpResponseMessage response = this._httpClient.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;
            return json;
        }

        private StringContent CreateStringContent(string fieldname, string value, int rowId)
        {
            var searchParameters = new SearchParameters
            {
                lq = string.Empty,
                aq = $"{fieldname}=={value.Replace(" -", string.Empty)}* AND @rowId>{rowId}",
                sortCriteria = "@rowId ascending",
                numberOfResults = 80000
            };

            var json = JsonConvert.SerializeObject(searchParameters);
            return new StringContent(json, Encoding.UTF8, "application/json");
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
