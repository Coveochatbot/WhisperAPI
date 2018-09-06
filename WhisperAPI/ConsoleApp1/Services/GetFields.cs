using System.Net.Http;
using System.Net.Http.Headers;
using ConsoleApp1.Models.Facets;
using Newtonsoft.Json;

namespace ConsoleApp1.Services
{
    public class GetFields
    {
        private const string URL = "https://cloudplatform.coveo.com/rest/search/v2/fields";
        private readonly string _apiKey = "";
        private readonly HttpClient _httpClient = new HttpClient();

        public GetFields()
        {
            this.InitHttpClient();
        }

        public Fields Get()
        {
            return JsonConvert.DeserializeObject<Fields>(this.GetStringFromGet(URL));
        }

        private string GetStringFromGet(string url)
        {
            HttpResponseMessage response = this._httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var test = response.Content.ReadAsStringAsync().Result;
            return test;
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
