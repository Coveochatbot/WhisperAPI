using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Services
{
    public class NlpCall : INlpCall
    {
        private readonly string _baseAdress;

        private readonly HttpClient _httpClient;

        public NlpCall(HttpClient httpClient, string baseAdress)
        {
            this._httpClient = httpClient;
            this._baseAdress = baseAdress;
        }

        public NlpAnalysis GetNlpAnalysis(string sentence)
        {
            this.InitHttpClient();
            var response = this._httpClient.PostAsync("NLP/Analyse", CreateStringContent(sentence)).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<NlpAnalysis>(response.Content.ReadAsStringAsync().Result);
        }

        private static StringContent CreateStringContent(string sentence)
        {
            return new StringContent($"{{\"sentence\": \"{sentence}\"}}", Encoding.UTF8, "application/json");
        }

        private void InitHttpClient()
        {
            this._httpClient.BaseAddress = new Uri(this._baseAdress);
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
