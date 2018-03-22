using System.Net.Http;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry()
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>();
            this.For<IIndexSearch>().Use<IndexSearch>();
            this.For<IAPIKeyProvider>().Use<APIKeyProvider>();
            this.For<IHttpClientWrapper>().Use<HttpClientWrapper>();
        }
    }
}
