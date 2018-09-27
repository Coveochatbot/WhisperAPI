using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public class DocumentFacets : IDocumentFacets
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _baseAddress;

        private readonly HttpClient _httpClient;

        public DocumentFacets(HttpClient httpClient, string baseAddress)
        {
            this._httpClient = httpClient;
            this._baseAddress = baseAddress;
            this.InitHttpClient();
        }

        public List<Question> GetQuestions(IEnumerable<string> suggestedDocumentsUri)
        {
            var response = this._httpClient.PostAsync("ML/Analyze", CreateStringContent(suggestedDocumentsUri)).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<List<Question>>(response.Content.ReadAsStringAsync().Result);
        }

        private static StringContent CreateStringContent(IEnumerable<string> suggestedDocumentsUri)
        {
            var json = JsonConvert.SerializeObject(suggestedDocumentsUri.ToList());
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
