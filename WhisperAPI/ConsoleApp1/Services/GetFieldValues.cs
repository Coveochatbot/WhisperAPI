using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ConsoleApp1.Models.Facets_value;
using Newtonsoft.Json;

namespace ConsoleApp1.Services
{
    public class GetFieldValues
    {
        private const string URL = "https://cloudplatform.coveo.com/rest/search/v2/values";
        private readonly string _apiKey = "";
        private readonly HttpClient _httpClient = new HttpClient();

        public GetFieldValues()
        {
            this.InitHttpClient();
        }

        public FieldValues Get(string fieldName)
        {
            return JsonConvert.DeserializeObject<FieldValues>(this.GetStringFromPost(URL, this.CreateStringContent(fieldName)));
        }

        private string GetStringFromPost(string url, StringContent content)
        {
            HttpResponseMessage response = this._httpClient.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();

            var test = response.Content.ReadAsStringAsync().Result;
            return test;
        }

        private StringContent CreateStringContent(string fieldName)
        {
            return new StringContent($"{{\"field\": \"{fieldName}\"}}", Encoding.UTF8, "application/json");
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
