using WhisperAPI.Models.MLAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class FacetBuilder
    {
        private string _name;

        private string _value;

        public static FacetBuilder Build => new FacetBuilder();

        public Facet Instance => new Facet
        {
            Name = this._name,
            Value = this._value
        };

        private FacetBuilder()
        {
            this._name = "name";
            this._value = "value";
        }

        public FacetBuilder WithName(string name)
        {
            this._name = name;
            return this;
        }

        public FacetBuilder WithValue(string value)
        {
            this._value = value;
            return this;
        }
    }
}
