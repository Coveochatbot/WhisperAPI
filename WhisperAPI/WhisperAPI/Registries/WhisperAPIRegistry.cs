using System.Net.Http;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry(string apiKey)
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>();
            this.For<IIndexSearch>().Use<IndexSearch>().Ctor<string>("apiKey").Is(apiKey);
            this.For<HttpClient>().Use<HttpClient>();
        }
    }
}
