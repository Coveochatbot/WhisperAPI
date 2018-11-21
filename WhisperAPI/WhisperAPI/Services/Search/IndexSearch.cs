﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models.Search;

namespace WhisperAPI.Services.Search
{
    public class IndexSearch : IIndexSearch
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _apiKey;
        private readonly string _organizationID;
        private readonly int _numberOfResults;
        private readonly HttpClient _httpClient;
        private readonly string _searchEndPoint = "rest/search/v2?organizationId=";

        public IndexSearch(string apiKey, int numberOfResults, HttpClient client, string searchBaseAddress, string organizationID)
        {
            this._apiKey = apiKey;
            this._organizationID = organizationID;
            this._numberOfResults = numberOfResults;
            this._httpClient = client;
            this.InitHttpClient(searchBaseAddress);
        }

        public ISearchResult Search(string query)
        {
            return JsonConvert.DeserializeObject<SearchResult>(this.GetStringFromPost(this._searchEndPoint + this._organizationID, this.CreateStringContent(query)));
        }

        private string GetStringFromPost(string url, StringContent content)
        {
            Log.Info(string.Concat("SEARCH URL : ", url));
            HttpResponseMessage response = this._httpClient.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }

        private StringContent CreateStringContent(string query)
        {
            var searchParameters = new SearchParameters
            {
                Q = query,
                NumberOfResults = this._numberOfResults
            };

            var json = JsonConvert.SerializeObject(searchParameters);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private void InitHttpClient(string baseURL)
        {
            this._httpClient.BaseAddress = new System.Uri(baseURL);

            // Add an Accept header for JSON format.
            this._httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // Add an Authorization header with the ApiKey
            this._httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this._apiKey}");
        }
    }
}
