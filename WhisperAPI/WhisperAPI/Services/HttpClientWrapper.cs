using System.Net.Http;
using System.Net.Http.Headers;

namespace WhisperAPI.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IAPIKeyProvider _apiKeyProvider;

        public HttpClientWrapper(IAPIKeyProvider apiKeyProvider)
        {
            this._apiKeyProvider = apiKeyProvider;
        }

        public string GetStringFromPost(string url, StringContent content)
        {
            HttpResponseMessage response = this.CreateHttpClient().PostAsync(url, content).Result;

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

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();

            // Add an Accept header for JSON format.
            httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // Add an Authorization header with the ApiKey
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this._apiKeyProvider.GetAPIKey()}");

            return httpClient;
        }
    }
}
