using System;
using WhisperAPI.Models;

namespace WhisperAPI.Tests.Data.Builders
{
    public class SuggestedDocumentBuilder
    {
        private Guid _id;

        private string _title;

        private string _uri;

        private string _printableUri;

        private string _summary;

        private string _excerpt;

        public static SuggestedDocumentBuilder Build => new SuggestedDocumentBuilder();

        public SuggestedDocument Instance => new SuggestedDocument
        {
            Id = this._id,
            Summary = this._summary,
            Title = this._title,
            Uri = this._uri,
            PrintableUri = this._printableUri,
            Excerpt = this._excerpt
        };

        public SuggestedDocumentBuilder()
        {
            this._id = Guid.NewGuid();
            this._summary = "Summary";
            this._uri = "Uri";
            this._excerpt = "Excerpt";
            this._printableUri = "PrintableUri";
            this._title = "Title";
        }

        public SuggestedDocumentBuilder WithId(Guid id)
        {
            this._id = id;
            return this;
        }

        public SuggestedDocumentBuilder WithTitle(string title)
        {
            this._title = title;
            return this;
        }

        public SuggestedDocumentBuilder WithSummary(string summary)
        {
            this._summary = summary;
            return this;
        }

        public SuggestedDocumentBuilder WithExcerpt(string excerpt)
        {
            this._excerpt = excerpt;
            return this;
        }

        public SuggestedDocumentBuilder WithUri(string uri)
        {
            this._uri = uri;
            return this;
        }

        public SuggestedDocumentBuilder WithPrintableUri(string printableUri)
        {
            this._printableUri = printableUri;
            return this;
        }
    }
}
