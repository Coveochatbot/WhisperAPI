using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Services.NLPAPI
{
    public class NlpCall : INlpCall
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _baseAddress;

        private readonly HttpClient _httpClient;

        public NlpCall(HttpClient httpClient, string baseAddress)
        {
            this._httpClient = httpClient;
            this._baseAddress = baseAddress;
            this.InitHttpClient();
        }

        public NlpAnalysis GetNlpAnalysis(string sentence)
        {
            var response = this._httpClient.PostAsync("NLP/Analyze", CreateStringContent(sentence)).Result;
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<NlpAnalysis>(response.Content.ReadAsStringAsync().Result);
        }

        private static StringContent CreateStringContent(string sentence)
        {
            return new StringContent($"{{\"sentence\": \"{sentence}\"}}", Encoding.UTF8, "application/json");
        }

        private void InitHttpClient()
        {
            this._httpClient.BaseAddress = new Uri(this._baseAddress);
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
