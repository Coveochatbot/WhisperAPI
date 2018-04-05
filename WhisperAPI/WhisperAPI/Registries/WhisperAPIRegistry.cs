using System.Collections.Generic;
using System.Net.Http;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry(string apiKey, List<string> irrelevantsIntents, string nlpApiBaseAdress)
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>().Ctor<List<string>>("irrelevantsIntents").Is(irrelevantsIntents);
            this.For<INlpCall>().Use<NlpCall>().Ctor<string>("baseAdress").Is(nlpApiBaseAdress);
            this.For<IIndexSearch>().Use<IndexSearch>().Ctor<string>("apiKey").Is(apiKey);
            this.For<HttpClient>().Use<HttpClient>();
        }
    }
}
