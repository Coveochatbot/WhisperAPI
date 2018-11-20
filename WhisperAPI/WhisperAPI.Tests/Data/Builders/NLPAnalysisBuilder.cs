using System.Collections.Generic;
using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class NlpAnalysisBuilder
    {
        private List<Intent> _intents;

        private List<Entity> _entities;

        public static NlpAnalysisBuilder Build => new NlpAnalysisBuilder();

        public NlpAnalysis Instance => new NlpAnalysis
        {
            Intents = this._intents,
            Entities = this._entities,
            ParsedQuery = "fils couleur 6"
        };

        private NlpAnalysisBuilder()
        {
            this._intents = new List<Intent>();
            this._entities = new List<Entity>();
        }

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
    }
}
