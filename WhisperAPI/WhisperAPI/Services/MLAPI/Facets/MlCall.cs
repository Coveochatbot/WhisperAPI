using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models;
using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Services.MLAPI.Facets
{
    public class MlCall : IMlCall
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _baseAdress;

        private readonly HttpClient _httpClient;

        public MlCall(HttpClient httpClient, string baseAdress)
        {
            this._httpClient = httpClient;
            this._baseAdress = baseAdress;
            this.InitHttpClient();
        }

        public FacetAnalysis GetFacetAnalysis(IEnumerable<SuggestedDocument> suggestedDocuments)
        {
            var response = this._httpClient.PostAsync("ML/Analyze", CreateStringContent(suggestedDocuments)).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<FacetAnalysis>(response.Content.ReadAsStringAsync().Result);
        }

        private static StringContent CreateStringContent(IEnumerable<SuggestedDocument> suggestedDocuments)
        {
            var json = JsonConvert.SerializeObject(suggestedDocuments.ToList());
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private void InitHttpClient()
        {
            this._httpClient.BaseAddress = new Uri(this._baseAdress);
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
