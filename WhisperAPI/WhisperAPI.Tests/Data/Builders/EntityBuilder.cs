using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class EntityBuilder
    {
        private string _name = "EntityName";

        public EntityBuilder WithName(string name)
        {
            this._name = name;
            return this;
        }

        public Entity Build()
        {
            return new Entity
            {
                Name = this._name
            };
        }
    }
}
