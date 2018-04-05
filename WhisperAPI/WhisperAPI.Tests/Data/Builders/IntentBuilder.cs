using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class IntentBuilder
    {
        private string _name = "IntentName";

        private double _confidence = 1;

        public IntentBuilder WithName(string name)
        {
            this._name = name;
            return this;
        }

        public IntentBuilder WithConfidence(double confidence)
        {
            this._confidence = confidence;
            return this;
        }

        public Intent Build() => new Intent
        {
            Name = this._name,
            Confidence = this._confidence
        };
    }
}
