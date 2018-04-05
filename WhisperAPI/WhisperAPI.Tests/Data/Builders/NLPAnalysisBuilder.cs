using System.Collections.Generic;
using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class NlpAnalysisBuilder
    {
        private List<Intent> _intents = new List<Intent>();

        private List<Entity> _entities = new List<Entity>();

        public NlpAnalysisBuilder WithIntents(List<Intent> intents)
        {
            this._intents = intents;
            return this;
        }

        public NlpAnalysisBuilder WithEntities(List<Entity> entities)
        {
            this._entities = entities;
            return this;
        }

        public NlpAnalysis Build() => new NlpAnalysis
        {
            Intents = this._intents,
            Entities = this._entities
        };
    }
}
