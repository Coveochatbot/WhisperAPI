using WhisperAPI.Models.NLPAPI;

namespace WhisperAPI.Tests.Data.Builders
{
    public class IntentBuilder
    {
        private string _name;

        private double _confidence;

        public static IntentBuilder Build => new IntentBuilder();

        public Intent Instance => new Intent
        {
            Name = this._name,
            Confidence = this._confidence
        };

        private IntentBuilder()
        {
            this._name = "IntenName";
            this._confidence = 1;
        }

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
    }
}
