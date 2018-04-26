using System;
using System.Collections.Generic;
using System.Net.Http;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry(string apiKey, List<string> irrelevantIntents, string nlpApiBaseAdress, string contextLifeSpan, string searchEndPoint)
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>().Ctor<List<string>>("irrelevantIntents").Is(irrelevantIntents);
            this.For<INlpCall>().Use<NlpCall>().Ctor<string>("baseAdress").Is(nlpApiBaseAdress);
            this.For<IIndexSearch>().Use<IndexSearch>().Ctor<string>("apiKey").Is(apiKey).Ctor<string>("searchEndPoint").Is(searchEndPoint);
            this.For<HttpClient>().Use<HttpClient>();
            this.For<IContexts>().Singleton().Use<InMemoryContexts>()
                .Ctor<TimeSpan>("contextLifeSpan").Is(TimeSpan.Parse(contextLifeSpan));
        }
    }
}
