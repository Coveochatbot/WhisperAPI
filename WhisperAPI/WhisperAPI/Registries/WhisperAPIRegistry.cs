using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using WhisperAPI.Services;

namespace WhisperAPI.Registries
{
    public class WhisperApiRegistry : StructureMap.Registry
    {
        public WhisperApiRegistry(string apiKey, List<string> irrelevantIntents, string nlpApiBaseAdress, string contextLifeSpan)
        {
            this.For<ISuggestionsService>().Use<SuggestionsService>().Ctor<List<string>>("irrelevantIntents").Is(irrelevantIntents);
            this.For<INlpCall>().Use<NlpCall>().Ctor<string>("baseAdress").Is(nlpApiBaseAdress);
            this.For<IIndexSearch>().Use<IndexSearch>().Ctor<string>("apiKey").Is(apiKey);
            this.For<HttpClient>().Use<HttpClient>();
            this.For<Contexts>().Use<Contexts>()
                .Ctor<DbContextOptions<Contexts>>("options").Is(new DbContextOptionsBuilder<Contexts>().UseInMemoryDatabase("contextDB").Options)
                .Ctor<TimeSpan>("contextLifeSpan").Is(TimeSpan.Parse(contextLifeSpan));
        }
    }
}
