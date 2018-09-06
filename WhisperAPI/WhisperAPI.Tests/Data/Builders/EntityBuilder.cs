using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class EntityBuilder
    {
        private string _name;

        public static EntityBuilder Build => new EntityBuilder();

        public Entity Instance => new Entity
        {
            Name = this._name
        };

        private EntityBuilder()
        {
            this._name = "EntityName";
        }

        public EntityBuilder WithName(string name)
        {
            this._name = name;
            return this;
        }
    }
}