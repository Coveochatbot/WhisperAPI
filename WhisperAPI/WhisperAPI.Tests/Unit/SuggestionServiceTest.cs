using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using WhisperAPI.Models;
using WhisperAPI.Services;

namespace WhisperAPI.Tests.Unit
{
    [TestFixture]
    public class SuggestionServiceTest
    {
        private ISuggestionsService _suggestionsServiceValid;
        private ISuggestionsService _suggestionsServiceEmpty;
        private ISuggestionsService _suggestionsServiceNull;

        private Mock<IIndexSearch> _indexSearchValid;
        private Mock<IIndexSearch> _indexSearchEmpty;
        private Mock<IIndexSearch> _indexSearchNull;

        private List<SuggestedDocument> _suggestedDocumentOk;

        [SetUp]
        public void SetUp()
        {
            var searchResult = new SearchResult
            {
                NbrElements = 4,
                Elements = new List<SearchResultElement>
                {
                    new SearchResultElement
                    {
                        Title = "Available Coveo Cloud V2 Source Types",
                        Uri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                        PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                        Summary = null,
                        Score = 4280
                    },
                    new SearchResultElement
                    {
                        Title = "Coveo Cloud Query Syntax Reference",
                        Uri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                        PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                        Summary = null,
                        Score = 3900
                    },
                    new SearchResultElement
                    {
                        Title = "Events",
                        Uri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                        PrintableUri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                        Summary = null,
                        Score = 2947
                    },
                    new SearchResultElement
                    {
                        Title = "Coveo Facet Component (CoveoFacet)",
                        Uri = "https://coveo.github.io/search-ui/components/facet.html",
                        PrintableUri = "https://coveo.github.io/search-ui/components/facet.html",
                        Summary = null,
                        Score = 2932
                    }
                }
            };

            this._suggestedDocumentOk = new List<SuggestedDocument>
            {
                new SuggestedDocument
                {
                    Title = "Available Coveo Cloud V2 Source Types",
                    Uri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                    PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Available_Coveo_Cloud_V2_Source_Types.htm",
                    Summary = null
                },
                new SuggestedDocument
                {
                    Title = "Coveo Cloud Query Syntax Reference",
                    Uri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                    PrintableUri = "https://onlinehelp.coveo.com/en/cloud/Coveo_Cloud_Query_Syntax_Reference.htm",
                    Summary = null
                },
                new SuggestedDocument
                {
                    Title = "Events",
                    Uri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                    PrintableUri = "https://developers.coveo.com/display/JsSearchV1/Page/27230520/27230472/27230573",
                    Summary = null
                },
                new SuggestedDocument
                {
                    Title = "Coveo Facet Component (CoveoFacet)",
                    Uri = "https://coveo.github.io/search-ui/components/facet.html",
                    PrintableUri = "https://coveo.github.io/search-ui/components/facet.html",
                    Summary = null
                }
            };

            this._indexSearchValid = new Mock<IIndexSearch>();
            this._indexSearchValid
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns(searchResult);

            this._indexSearchEmpty = new Mock<IIndexSearch>();
            this._indexSearchEmpty
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns((ISearchResult)null);

            this._indexSearchNull = new Mock<IIndexSearch>();
            this._indexSearchNull
                .Setup(x => x.Search(It.IsAny<string>()))
                .Returns((ISearchResult)null);

            this._suggestionsServiceValid = new SuggestionsService(this._indexSearchValid.Object);
            this._suggestionsServiceEmpty = new SuggestionsService(this._indexSearchEmpty.Object);
            this._suggestionsServiceNull = new SuggestionsService(this._indexSearchNull.Object);
        }

        [Test]
        [TestCase("test")]
        public void Receive_valid_searchresult_from_search_then_return_list_of_suggestedDocuments(string suggestion)
        {
            this._suggestionsServiceValid.GetSuggestion(suggestion).Should().BeEquivalentTo(this._suggestedDocumentOk);
        }

        [Test]
        [TestCase("test")]
        public void Receive_empty_searchresult_from_search_then_return_empty_list_of_suggestedDocuments(string suggestion)
        {
            this._suggestionsServiceEmpty.GetSuggestion(suggestion).Should().BeEquivalentTo(new List<SuggestedDocument>());
        }

        [Test]
        [TestCase("test")]
        public void Receive_null_searchresult_from_search_then_return_empty_list_of_suggestedDocuments(string suggestion)
        {
            this._suggestionsServiceNull.GetSuggestion(suggestion).Should().BeEquivalentTo(new List<SuggestedDocument>());
        }
    }
}
