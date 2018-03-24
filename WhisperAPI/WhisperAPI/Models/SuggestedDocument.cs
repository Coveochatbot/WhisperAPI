namespace WhisperAPI.Models
{
    public class SuggestedDocument
    {
        public SuggestedDocument()
        {
        }

        public SuggestedDocument(ISearchResultElement searchResultElement)
        {
            this.Title = searchResultElement.Title;
            this.Uri = searchResultElement.Uri;
            this.PrintableUri = searchResultElement.PrintableUri;
            this.Summary = searchResultElement.Summary;
        }

        public string Title { get; set; }

        public string Uri { get; set; }

        public string PrintableUri { get; set; }

        public string Summary { get; set; }
    }
}
