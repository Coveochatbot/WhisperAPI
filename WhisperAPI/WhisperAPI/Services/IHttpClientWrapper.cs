using System.Net.Http;

namespace WhisperAPI.Services
{
    public interface IHttpClientWrapper
    {
        string GetStringFromPost(string url, StringContent content);
    }
}
