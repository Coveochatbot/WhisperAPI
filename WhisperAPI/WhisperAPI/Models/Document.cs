using System;
using WhisperAPI.Models.Search;

namespace WhisperAPI.Models
{
    public class Document
    {
        public Document()
        {
        }

        public Document(ISearchResultElement searchResultElement)
        {
            this.Id = Guid.NewGuid();
            this.Title = searchResultElement.Title;
            this.Uri = searchResultElement.Uri;
            this.PrintableUri = searchResultElement.PrintableUri;
            this.Summary = searchResultElement.Summary;
            this.Excerpt = searchResultElement.Excerpt;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Uri { get; set; }

        public string PrintableUri { get; set; }

        public string Summary { get; set; }

        public string Excerpt { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((Document)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Title != null ? this.Title.GetHashCode() : 0) * 397) ^ (this.Uri != null ? this.Uri.GetHashCode() : 0);
            }
        }

        protected bool Equals(Document other)
        {
            return string.Equals(this.Title, other.Title) && string.Equals(this.Uri, other.Uri);
        }
    }
}