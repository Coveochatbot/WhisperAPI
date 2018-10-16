using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public class FilterDocuments : IFilterDocuments
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _baseAddress;

        private readonly HttpClient _httpClient;

        public FilterDocuments(HttpClient httpClient, string baseAddress)
        {
            this._httpClient = httpClient;
            this._baseAddress = baseAddress;
            this.InitHttpClient();
        }

        public List<string> FilterDocumentsByFacets(FilterDocumentsParameters parameters)
        {
            var response = this._httpClient.PostAsync("ML/Filter/Facets", CreateStringContent(parameters)).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);
        }

        private static StringContent CreateStringContent(FilterDocumentsParameters parameters)
        {
            var json = JsonConvert.SerializeObject(parameters);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private void InitHttpClient()
        {
            this._httpClient.BaseAddress = new Uri(this._baseAddress);
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
