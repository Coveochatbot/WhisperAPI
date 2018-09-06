namespace ConsoleApp1.Models.Search
{
    public interface ISearchResultElement
    {
        string title { get; set; }

        string clickUri { get; set; }

        string printableUri { get; set; }

        string summary { get; set; }

        string excerpt { get; set; }

        string uri { get; set; }

        string uniqueId { get; set; }

        RawInformation rawInformation { get; set; }
    }
}
