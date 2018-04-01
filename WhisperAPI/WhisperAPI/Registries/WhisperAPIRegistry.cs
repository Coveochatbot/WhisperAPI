using System.Collections.Generic;
using System.Net.Http;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry(string apiKey, List<string> intents, string nlpApiBaseAdress)
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>().Ctor<List<string>>("intents").Is(intents);
            this.For<INlpCall>().Use<NlpCall>().Ctor<string>("baseAdress").Is(nlpApiBaseAdress);
            this.For<IIndexSearch>().Use<IndexSearch>().Ctor<string>("apiKey").Is(apiKey);
            this.For<HttpClient>().Use<HttpClient>();
        }
    }
}
