using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SearchResultElementBuilder
    {
        private string _title = "Title";

        private string _uri = "Uri";

        private string _printableUri = "PrintableUri";

        private string _summary = "Summary";

        private int _score = 1;

        public SearchResultElementBuilder WithTitle(string title)
        {
            this._title = title;
            return this;
        }

        public SearchResultElementBuilder WithUri(string uri)
        {
            this._uri = uri;
            return this;
        }

        public SearchResultElementBuilder WithPrintableUri(string printableUri)
        {
            this._printableUri = printableUri;
            return this;
        }

        public SearchResultElementBuilder WithSummary(string summary)
        {
            this._summary = summary;
            return this;
        }

        public SearchResultElementBuilder WithScore(int score)
        {
            this._score = score;
            return this;
        }

        public SearchResultElement Build() => new SearchResultElement
        {
            PrintableUri = this._printableUri,
            Summary = this._summary,
            Title = this._title,
            Uri = this._uri,
            Score = this._score
        };
}
}
