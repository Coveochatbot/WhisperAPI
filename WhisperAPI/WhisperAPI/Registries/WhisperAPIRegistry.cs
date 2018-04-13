using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry(string apiKey, List<string> irrelevantsIntents, string nlpApiBaseAdress, string contextLifeSpan)
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>().Ctor<List<string>>("irrelevantsIntents").Is(irrelevantsIntents);
            this.For<INlpCall>().Use<NlpCall>().Ctor<string>("baseAdress").Is(nlpApiBaseAdress);
            this.For<IIndexSearch>().Use<IndexSearch>().Ctor<string>("apiKey").Is(apiKey);
            this.For<HttpClient>().Use<HttpClient>();
            this.For<Contexts>().Use<Contexts>()
                .Ctor<DbContextOptions<Contexts>>("options").Is(new DbContextOptionsBuilder<Contexts>().UseInMemoryDatabase("contextDB").Options)
                .Ctor<TimeSpan>("contextLifeSpan").Is(TimeSpan.Parse(contextLifeSpan));
            this.For<InMemoryContexts>().Use<InMemoryContexts>()
                .Ctor<TimeSpan>("contextLifeSpan").Is(TimeSpan.Parse(contextLifeSpan));
        }
    }
}
